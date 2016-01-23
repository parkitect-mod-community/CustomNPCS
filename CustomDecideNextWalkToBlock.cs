using System;
using UnityEngine;
using BehaviourTree;
using System.Collections.Generic;

namespace HelloMod
{
	public class CustomDecideNextBlockToWalk :  DecideNextWalkToBlockAction
	{

		private Person _person;

		private Block[] possibleNormalBlocks = new Block[3];

		private Block[] possibleInterestingBlocks = new Block[3];

		private float[] possibleInterestingBlocksInterestFactor = new float[3];

		private Block[] possibleProhibitedBlocks = new Block[3];

		private string _outblock;
		private string _reward;
		public CustomDecideNextBlockToWalk (string outblock,string reward) : base(outblock)
		{
			_reward = reward;
			_outblock = outblock;
		}

		public override void initialize (BehaviourTree.DataContext dataContext)
		{
			this._person = dataContext.person;

			base.initialize (dataContext);
		}

		protected override Result run (BehaviourTree.DataContext dataContext)
		{
			Block block = null;
			Block block2 = null;
			if (this._person.currentBlock == null)
			{
				return Node.Result.FAILED;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Vector3 lhs = this._person.transform.forward;
			if (this._person.previousBlock != null)
			{
				lhs = this._person.currentBlock.transform.position - this._person.previousBlock.transform.position;
				lhs.y = 0f;
				lhs.Normalize();
			}

			BlockNeighbour[] connected = this._person.currentBlock.getConnected();

			List<float> weights = new List<float> ();
			List<Block> QLearningBlock = new List<Block>();
			float max_weight = 0;

			for (int i = 0; i < connected.Length; i++)
			{
				BlockNeighbour blockNeighbour = connected[i];
				Block connect_blocks = blockNeighbour.block;
				bool cantWanderOntBlock = false;
				if (!this._person.canWanderOnto (connect_blocks)) {
					cantWanderOntBlock = true;
				}

				Vector3 rhs = connect_blocks.transform.position - this._person.currentBlock.transform.position;
				rhs.y = 0f;
				rhs.Normalize();

			

				if (Vector3.Dot(lhs, rhs) < -0.5f)
				{
					if (!cantWanderOntBlock)
					{
						block = connect_blocks;
					}
					else
					{
						block2 = connect_blocks;
					}

	
				}
				else if (connect_blocks is Path && this.canStepOntoPathWithoutThinking((Path)connect_blocks))
				{
					if (cantWanderOntBlock)
					{
						this.possibleProhibitedBlocks[num3] = connect_blocks;
						num3++;
					}
					else if (num < 3)
					{
						this.possibleNormalBlocks[num] = connect_blocks;
						num++;
					}

					var current = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, Mathf.FloorToInt (_person.transform.position.x), Mathf.RoundToInt (_person.transform.position.y), Mathf.FloorToInt (_person.transform.position.z),_person.currentBlock);
					var future = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, Mathf.FloorToInt (connect_blocks.transform.position.x), Mathf.RoundToInt (connect_blocks.transform.position.y), Mathf.FloorToInt (connect_blocks.transform.position.z),connect_blocks);
					float potentialReward = current.findMaxUtility (future);
					max_weight += potentialReward ;
					weights.Add (max_weight);
					QLearningBlock.Add (connect_blocks);


	
				}
				else
				{
					float num4 = 0f;
					PersonBehaviour[] components = this._person.GetComponents<PersonBehaviour>();
					for (int j = 0; j < components.Length; j++)
					{
						PersonBehaviour personBehaviour = components[j];
						if (personBehaviour is WalkToInterestCalculator)
						{
							float num5 = (personBehaviour as WalkToInterestCalculator).calculateInterest(connect_blocks, this._person);
							if (num5 > num4)
							{
								num4 = num5;
							}
						}
					}
					if (num4 > 0f && UnityEngine.Random.value < num4 && !(connect_blocks is Exit))
					{
						this.possibleInterestingBlocks[num2] = connect_blocks;
						possibleInterestingBlocksInterestFactor [num2] = num4;
						num2++;
					}
				}
			}

	
		
				
			if (num <= 0 && num3 > 0 && block == null) {
				this.possibleNormalBlocks = this.possibleProhibitedBlocks;
				num = num3;
				block = block2;
			}

			if (num2 > 0) {
				int selectedBlock = UnityEngine.Random.Range (0, num2);
				block = this.possibleInterestingBlocks [selectedBlock];
				dataContext.set (this._reward, possibleInterestingBlocksInterestFactor [selectedBlock]);

			}  
			else if (QLearningBlock != null && ((Guest)_person).visitingState == Guest.VisitingState.IN_PARK && UnityEngine.Random.value <= .6f) {
				//UnityEngine.Debug.Log ("Decided to step on block because of potential reward:" + reward);

				int index = weights.BinarySearch (UnityEngine.Random.value * max_weight);
				block = QLearningBlock [index];
				dataContext.set (this._reward, 0.0f);
			

			}
			else if (num > 1)
			{
				block = this.possibleNormalBlocks[UnityEngine.Random.Range(0, num)];
			}
			else if (num == 1) {
				block = this.possibleNormalBlocks [UnityEngine.Random.Range (0, num)];

			}
				


			if (block == null) {
				if (this._person.currentBlock != null) {
					block = this._person.currentBlock;

				} else {
					//when the guest turns around
					block = _person.previousBlock;
				}
			}

			
			dataContext.set(this._outblock, block);
			return Node.Result.SUCCESS;
		}

		private bool canStepOntoPathWithoutThinking(Path path)
		{
			return !(this._person is Guest) || !(path is Queue) || (path as Queue).getStationController() == null || this._person.currentBlock is Queue;
		}
	}
}

