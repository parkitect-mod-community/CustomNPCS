using System;
using BehaviourTree;
using System.Collections.Generic;

namespace HelloMod
{
	public class WanderAction : Node
	{
		private const float STICK_FACTOR = 4.0f;
		private const int MAX_DEPTH = 5;

		private List<Block> _blocks = new List<Block>();
		private Person _person;

		public WanderAction ()
		{
			
		}

		public override void initialize (DataContext dataContext)
		{
			_person = dataContext.person;
			base.initialize (dataContext);
		}

		protected override Result run (DataContext dataContext)
		{
			while (true) {
				BlockNeighbour[] connected = _person.currentBlock.getConnected ();
				for (int i = 0; i < connected.Length; i++) {
					//connected [i].block;
				}
			}
			return Result.SUCCESS;
		}
	}
}

