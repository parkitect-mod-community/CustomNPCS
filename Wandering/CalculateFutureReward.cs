using System;
using BehaviourTree;
using UnityEngine;

namespace ImprovedNPC.Wandering
{
	public class CalculateFutureReward :Node
	{
		private string _block;
		private string _reward;
		private Person _person;
		private string _bounce;
		public CalculateFutureReward (string reward,string block,string bounce)
		{
			_block = block;
			_reward = reward;
			_bounce = bounce;
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

			var next = dataContext.get<Block> (_block);
			bool bounce = dataContext.get<bool> (_bounce);

			int futureX = Mathf.FloorToInt (next.transform.position.x);
			int futureY = Mathf.RoundToInt (next.transform.position.y);
			int futureZ = Mathf.FloorToInt (next.transform.position.z);

			var curentNode = QLearningCache.Instance.GetNode (ImprovedNPC.GUEST_QLEARNING, currentX, currentY, currentZ,_person.currentBlock,_person);
			var futureNode = QLearningCache.Instance.GetNode (ImprovedNPC.GUEST_QLEARNING, futureX, futureY, futureZ,next,_person);
			curentNode.calculateNewState (futureNode, dataContext.get<float>(_reward),bounce);

			return Result.SUCCESS;
		}
	}
}

