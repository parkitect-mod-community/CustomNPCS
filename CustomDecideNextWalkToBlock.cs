using System;
using UnityEngine;
using BehaviourTree;

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
			float reward = 0.0f;
			Block QLearningBlock = null;


			for (int i = 0; i < connected.Length; i++)
			{
				BlockNeighbour blockNeighbour = connected[i];
				Block block3 = blockNeighbour.block;
				bool flag = false;
				if (!this._person.canWanderOnto (block3)) {
					flag = true;


				} 

				Vector3 rhs = block3.transform.position - this._person.currentBlock.transform.position;
				rhs.y = 0f;
				rhs.Normalize();
				if (Vector3.Dot(lhs, rhs) < -0.5f)
				{
					if (!flag)
					{
						block = block3;
					}
					else
					{
						block2 = block3;
					}

	
				}
				else if (block3 is Path && this.canStepOntoPathWithoutThinking((Path)block3))
				{
					if (flag)
					{
						this.possibleProhibitedBlocks[num3] = block3;
						num3++;
					}
					else if (num < 3)
					{
						this.possibleNormalBlocks[num] = block3;
						num++;
					}

			
					var current = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, Mathf.FloorToInt (_person.transform.position.x), Mathf.RoundToInt (_person.transform.position.y), Mathf.FloorToInt (_person.transform.position.z));
					var future = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, Mathf.FloorToInt (block3.transform.position.x), Mathf.RoundToInt (block3.transform.position.y), Mathf.FloorToInt (block3.transform.position.z));
					float potentialReward = current.getValueBasedOnLocation (future);
					if ( potentialReward > reward) {
						QLearningBlock = block3;
						reward = potentialReward;

					}

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
							float num5 = (personBehaviour as WalkToInterestCalculator).calculateInterest(block3, this._person);
							if (num5 > num4)
							{
								num4 = num5;
							}
						}
					}
					if (num4 > 0f && UnityEngine.Random.value < num4 && !(block3 is Exit))
					{
						this.possibleInterestingBlocks[num2] = block3;
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
			else if (QLearningBlock != null && ((Guest)_person).visitingState == Guest.VisitingState.IN_PARK && UnityEngine.Random.value <= .8f) {
				UnityEngine.Debug.Log ("Decided to step on block because of potential reward:" + reward);
				dataContext.set (this._reward, 0.0f);
				block = QLearningBlock;

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
					block = this._person.previousBlock;
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

