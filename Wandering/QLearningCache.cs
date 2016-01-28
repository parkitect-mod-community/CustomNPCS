using  System.Collections.Generic;

namespace ImprovedNPC.Wandering
{
	public class QLearningCache
	{
		public class NodeState
		{

			public float LValue { get; set; }
			public float RValue { get; set; }
			public float FValue { get; set; }
			public float BValue { get; set; }

			public int X{get;private set;}
			public int Y {get;private set;}
			public int Z {get;private set;}

			private QLearningCache _cache;
			private string _cacheName;
			public NodeState(QLearningCache cache,int x,int y, int z,string cacheName, Block associatedBlock)
			{
				
				_cacheName = cacheName;
				_cache = cache;
				LValue = 0;
				RValue = 0;
				FValue = 0;
				BValue = 0;
				this.X = x;
				this.Y = y;
				this.Z = z;
			
				associatedBlock.OnDestroyed += () => {
					if(_cache.TryGetNode(_cacheName,X-1,Y,Z-1) != null)_cache.TryGetNode(_cacheName,X-1,Y,Z-1) .LValue = 0;
					if(_cache.TryGetNode(_cacheName,X+1,Y,Z-1) != null)_cache.TryGetNode(_cacheName,X+1,Y,Z-1) .RValue = 0;
					if(_cache.TryGetNode(_cacheName,X,Y+1,Z-1) != null)_cache.TryGetNode(_cacheName,X,Y+1,Z-1) .FValue = 0;
					if(_cache.TryGetNode(_cacheName,X,Y-1,Z-1) != null)_cache.TryGetNode(_cacheName,X,Y-1,Z-1) .BValue = 0;

					if(_cache.TryGetNode(_cacheName,X-1,Y,Z) != null)_cache.TryGetNode(_cacheName,X-1,Y,Z).LValue = 0;
					if(_cache.TryGetNode(_cacheName,X+1,Y,Z) != null)_cache.TryGetNode(_cacheName,X+1,Y,Z).RValue = 0;
					if(_cache.TryGetNode(_cacheName,X,Y+1,Z) != null)_cache.TryGetNode(_cacheName,X,Y+1,Z).FValue = 0;
					if(_cache.TryGetNode(_cacheName,X,Y-1,Z) != null)_cache.TryGetNode(_cacheName,X,Y-1,Z).BValue = 0;

					if(_cache.TryGetNode(_cacheName,X-1,Y,Z+1) != null)_cache.TryGetNode(_cacheName,X-1,Y,Z+1) .LValue = 0;
					if(_cache.TryGetNode(_cacheName,X+1,Y,Z+1) != null)_cache.TryGetNode(_cacheName,X+1,Y,Z+1) .RValue = 0;
					if(_cache.TryGetNode(_cacheName,X,Y+1,Z+1) != null)_cache.TryGetNode(_cacheName,X,Y+1,Z+1) .FValue = 0;
					if(_cache.TryGetNode(_cacheName,X,Y-1,Z+1) != null)_cache.TryGetNode(_cacheName,X,Y-1,Z+1) .BValue = 0;

					_cache.deleteNode(_cacheName,X,Y,Z);

				};
			}
				
			public float findMaxUtility(NodeState futureState)
			{
				float maxUtility = float.MinValue;
		
				int x_diff = (futureState.X - X);
				int z_diff = (futureState.Z - Z);

				if (futureState.RValue != 0 && futureState.RValue > maxUtility  && x_diff != -1) {
					maxUtility = futureState.RValue;
				}
				if (futureState.LValue != 0 && futureState.LValue > maxUtility && x_diff != 1  ) {
						maxUtility = futureState.LValue;
				}
				if (futureState.FValue != 0 &&futureState.FValue > maxUtility && z_diff != -1  ) {
						maxUtility = futureState.FValue;
				}
				if (futureState.BValue != 0 &&futureState.BValue > maxUtility  && z_diff != 1 ) {
						maxUtility = futureState.BValue;
				}

				if (maxUtility == float.MinValue) {
					maxUtility = 0;
				}

				return maxUtility;

			}

			public void calculateNewState(NodeState futureState,float reward)
			{
				reward += Config.MOVMENT_COST;
				int x_diff = (futureState.X - X);
				int z_diff = (futureState.Z - Z);


				float maxUtility = 0.0f;
				maxUtility = findMaxUtility (futureState);

				if (x_diff == 1) {
					//float estimatedReward = futureState.RValue;
					RValue += Config.LEARNING_RATE * (reward + Config.DISCOUNT_FACTOR * maxUtility - RValue);
			
				} else if (x_diff == -1) {
					//float estimatedReward = futureState.LValue;
					LValue += Config.LEARNING_RATE * (reward + Config.DISCOUNT_FACTOR * maxUtility - LValue);
			
				} else if (z_diff == 1) {
					//float estimatedReward = futureState.FValue;
					FValue += Config.LEARNING_RATE * (reward + Config.DISCOUNT_FACTOR * maxUtility - FValue);
				
				} else if (z_diff == -1) {
					//float estimatedReward = futureState.BValue;
					BValue += Config.LEARNING_RATE * (reward + Config.DISCOUNT_FACTOR * maxUtility - BValue);
		
				}

			}
		}




		private Dictionary<string, QLearningCache.NodeState[,,]> nodes = new Dictionary<string,  QLearningCache.NodeState[,,]>() ;

		private static QLearningCache instance;
		public static QLearningCache Instance{get{
				if (QLearningCache.instance == null) {
					QLearningCache.instance = new QLearningCache ();
				}
				return QLearningCache.instance;
			}}
		
		public QLearningCache ()
		{
            
			
		}

        public void ClearCache()
        {
            nodes.Clear ();
        }

		public void AddCache(string cacheName,int x, int y, int z)
		{
            nodes.Add (cacheName, new NodeState[x, y, z]);
		}

		public bool HasNode(string cacheName,int x, int y, int z)
		{
			var cache = nodes [cacheName];
			if(cache[x, y, z] == null)
				return false;
			return true;
		}

		public NodeState TryGetNode(string cacheName,int x, int y,int z)
		{
			var cache = nodes [cacheName];
			return cache [x, y, z];
		}

		public NodeState GetNode(string cacheName,int x, int y,int z,Block associatedBlock,IPathfindingAgent agent)
		{
				var cache = nodes [cacheName];
				if (cache [x, y, z] == null) {	
					cache [x, y, z] = new NodeState (this, x, y, z, cacheName, associatedBlock);

				var connected = associatedBlock.getConnected ();
				for (int i = 0; i < connected.Length; i++)
				{
					var connect_blocks = connected [i].block;
					if (agent.canUseForPathfinding (connect_blocks)) {
						var node = QLearningCache.Instance.GetNode (cacheName, (int)connect_blocks.intPosition.x, (int)connect_blocks.intPosition.y, (int)connect_blocks.intPosition.z, connect_blocks, agent);

						int x_diff = (node.X - x);
						int z_diff = (node.Z - z);
						if (x_diff == 1 ) {
							//float estimatedReward = futureState.RValue;
							node.RValue =.01f;
							cache [x, y, z].LValue = .01f;

						} else if (x_diff == -1) {
							//float estimatedReward = futureState.LValue;
							node.LValue = .01f;
							cache [x, y, z].RValue = .01f;
						} else if (z_diff == 1) {
							//float estimatedReward = futureState.FValue;
							node.FValue = .01f;
							cache [x, y, z].BValue = .01f;

						} else if (z_diff == -1) {
							//float estimatedReward = futureState.BValue;
							node.BValue = .01f;
							cache [x, y, z].FValue = .01f;

						}
					}


				}
			}


			return cache [x, y, z];
		}

		public void deleteNode(string cacheName,int x, int y,int z)
		{
			var cache = nodes [cacheName];
			cache[x, y, z] = null;

		}


		public QLearningCache.NodeState[,,] GetAllNodes(string cacheName)
		{
			return nodes [cacheName];
		}

	}
}

