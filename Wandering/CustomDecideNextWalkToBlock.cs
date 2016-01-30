using System;
using UnityEngine;
using BehaviourTree;
using System.Collections.Generic;

namespace ImprovedNPC.Wandering
{
	public class CustomDecideNextBlockToWalk :  DecideNextWalkToBlockAction
	{

		private Person _person;

		private string _outblock;
		private string _reward;
		private string _bounce;
		public CustomDecideNextBlockToWalk (string outblock,string reward,string bounce) : base(outblock)
		{
			_bounce = bounce;
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
			if (this._person.currentBlock == null || !QLearningCache.Instance.IsCacheAvailable(ImprovedNPC.GUEST_QLEARNING))
			{
				return Node.Result.FAILED;
			}
			Vector3 lhs = this._person.transform.forward;
			if (this._person.previousBlock != null)
			{
				lhs = this._person.currentBlock.transform.position - this._person.previousBlock.transform.position;
				lhs.y = 0f;
				lhs.Normalize();
			}

			BlockNeighbour[] connected = this._person.currentBlock.getConnected();

			WeightedPick<Block> cantWander = new WeightedPick<Block> ();
			WeightedPick<Block> canWander = new WeightedPick<Block> ();
			WeightedPick<Block> interestingBlocks = new WeightedPick<Block> ();
			bool bounce = true;
	
			Block block = null;
			float reward = 0.0f;

			for (int i = 0; i < connected.Length; i++)
			{
				BlockNeighbour blockNeighbour = connected[i];
				Block connect_blocks = blockNeighbour.block;
				bool canWanderOnto = this._person.canWanderOnto (connect_blocks);

				Vector3 rhs = connect_blocks.transform.position - this._person.currentBlock.transform.position;
				rhs.y = 0f;
				rhs.Normalize();

			

				if (Vector3.Dot(lhs, rhs) < -0.5f)
				{
					block = connect_blocks;
					reward = Config.BOUNCE_COST;
				}
				else if (connect_blocks is Path && this.canStepOntoPathWithoutThinking((Path)connect_blocks))
				{
					if (canWanderOnto)
					{
						var current = QLearningCache.Instance.GetNode (ImprovedNPC.GUEST_QLEARNING, Mathf.FloorToInt (_person.transform.position.x), Mathf.RoundToInt (_person.transform.position.y), Mathf.FloorToInt (_person.transform.position.z),_person.currentBlock,_person);
						var future = QLearningCache.Instance.GetNode (ImprovedNPC.GUEST_QLEARNING, (int)connect_blocks.intPosition.x,(int)connect_blocks.intPosition.y, (int)connect_blocks.intPosition.z,connect_blocks,_person);
						float potentialReward = current.findMaxUtility (future);
						canWander.Add (potentialReward , connect_blocks,Config.WANDERING_NEGATIVE_WEIGHT_FACTOR,Config.WANDERING_POSITIVE_WEIGHT_FACTOR);
					}
					else 
					{

						var current = QLearningCache.Instance.GetNode (ImprovedNPC.GUEST_QLEARNING, Mathf.FloorToInt (_person.transform.position.x), Mathf.RoundToInt (_person.transform.position.y), Mathf.FloorToInt (_person.transform.position.z),_person.currentBlock,_person);
						var future = QLearningCache.Instance.GetNode (ImprovedNPC.GUEST_QLEARNING, (int)connect_blocks.intPosition.x,(int)connect_blocks.intPosition.y, (int)connect_blocks.intPosition.z,connect_blocks,_person);
						float potentialReward = current.findMaxUtility (future);
						cantWander.Add (potentialReward, connect_blocks,Config.WANDERING_NEGATIVE_WEIGHT_FACTOR,Config.WANDERING_POSITIVE_WEIGHT_FACTOR);

					}

				}
				else
				{
					float max_interest = 0f;
					PersonBehaviour[] components = this._person.GetComponents<PersonBehaviour>();
					for (int j = 0; j < components.Length; j++)
					{
						PersonBehaviour personBehaviour = components[j];
						if (personBehaviour is WalkToInterestCalculator)
						{
							float interest = (personBehaviour as WalkToInterestCalculator).calculateInterest(connect_blocks, this._person);
							if (interest > max_interest)
							{
								max_interest = interest;
							}
						}
					}
					if (max_interest > 0f && !(connect_blocks is Exit))
					{
						interestingBlocks.Add (max_interest, connect_blocks,Config.FINDING_ATTRACTION_NEGATIVE_WEIGHT_FACTOR,Config.FINDING_ATTRACTION_POSITIVE_WEIGHT_FACTOR);
					}
				}
			}

	
		
			if (interestingBlocks.NumberOfPairs () >= 1) {
				var weightPair = interestingBlocks.RandomPick ();
				block = weightPair.Item;
				reward = weightPair.Weight;
				bounce = false;
			} else if (canWander.NumberOfPairs () >= 1) {
				block = canWander.RandomPick ().Item;
				reward = 0.0f;
				bounce = false;
			} else if(cantWander.NumberOfPairs() >= 1){
				block = cantWander.RandomPick ().Item;
				reward = 0.0f;
				bounce = false;
			}
	

			if (block == null) {
				if (this._person.currentBlock != null) {
					block = _person.currentBlock;

				} else {
					//when the guest turns around
					block = _person.previousBlock;
					reward = Config.BOUNCE_COST;
				}
			}
			dataContext.set (this._bounce, bounce);
			dataContext.set (this._reward, reward);
			dataContext.set(this._outblock, block);
			return Node.Result.SUCCESS;
		}

		private bool canStepOntoPathWithoutThinking(Path path)
		{
			return !(this._person is Guest) || !(path is Queue) || (path as Queue).getStationController() == null || this._person.currentBlock is Queue;
		}
	}
}

