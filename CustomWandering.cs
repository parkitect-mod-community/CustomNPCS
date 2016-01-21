using System;

namespace HelloMod
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
			
			return base.setupTree ();
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

