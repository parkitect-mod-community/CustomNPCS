using System;
using BehaviourTree;
using UnityEngine;

namespace ImprovedNPC.Wandering
{
	public class CustomWandering : RoamingBehaviour
	{
		public CustomWandering ()
		{
		}

		protected override void Initialize (bool isDeserialized)
		{
			base.Initialize (isDeserialized);
		}
		protected override BehaviourTree.Node setupTree ()
		{
			
			return new Loop(new Node[]
			{
				new CustomDecideNextBlockToWalk("block","reward"),
					new CalculateFutureReward("reward","block"),
				new TurnBlockIntoWalkToPositionAction("block", "position"),
				new WalkToPositionAction("position", false),
				new TriggerLongTermPlanAction()
			});
		}

		public override void Update ()
		{ 
			
		

			base.Update ();

		}

		public override string getDescription ()
		{
			return "custom Wandering";
		}
	}
}

