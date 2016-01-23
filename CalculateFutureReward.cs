using System;
using BehaviourTree;
using UnityEngine;

namespace HelloMod
{
	public class CalculateFutureReward :Node
	{
		private string _block;
		private string _reward;
		private Person _person;
		public CalculateFutureReward (string reward,string block)
		{
			_block = block;
			_reward = reward;
		}

		public override void initialize (BehaviourTree.DataContext dataContext)
		{
			this._person = dataContext.person;

			base.initialize (dataContext);
		}

		protected override Result run (DataContext dataContext)
		{
			
			int currentX = Mathf.FloorToInt (_person.transform.position.x);
			int currentY = Mathf.RoundToInt (_person.transform.position.y);
			int currentZ = Mathf.FloorToInt (_person.transform.position.z);

			bool bounce = false;
			var next = dataContext.get<Block> (_block);
			if (next == null) {
				next = this._person.previousBlock;
				bounce = true;
				dataContext.set(_block, next);
			}

			int futureX = Mathf.FloorToInt (next.transform.position.x);
			int futureY = Mathf.RoundToInt (next.transform.position.y);
			int futureZ = Mathf.FloorToInt (next.transform.position.z);

			var curentNode = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, currentX, currentY, currentZ,_person.currentBlock);
			var futureNode = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, futureX, futureY, futureZ,next);
			curentNode.calculateNewState (futureNode, dataContext.get<float>(_reward),bounce);

			return Result.SUCCESS;
		}
	}
}

