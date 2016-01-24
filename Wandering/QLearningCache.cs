using  System.Collections.Generic;
namespace ImprovedNPC.Wandering
{
	public class QLearningCache
	{
		public class NodeState
		{
			private const float LEARNING_RATE = .80f;
			private const float DISCOUNT_FACTOR = .6f;

			public float LValue { get; protected set; }
			public float RValue { get; protected set; }
			public float FValue { get; protected set; }
			public float BValue { get; protected set; }

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
				if (futureState == null) {
					return 0.0f;
				}
		
				if (futureState.RValue > maxUtility ) {
						maxUtility = futureState.RValue;
				}
				if (futureState.LValue > maxUtility  ) {
						maxUtility = futureState.LValue;
				}
				if (futureState.FValue > maxUtility) {
						maxUtility = futureState.FValue;
				}
				if (futureState.BValue > maxUtility ) {
						maxUtility = futureState.BValue;
				}
				return maxUtility;

			}

			public void calculateNewState(NodeState futureState,float reward,bool IsBounce)
			{
				reward -= 0.04f;
				int x_diff = (futureState.X - X);
				int z_diff = (futureState.Z - Z);


				float maxUtility = 0.0f;
				if (!IsBounce) {
					maxUtility = findMaxUtility (futureState);
				} else {
					x_diff *= -1;
					z_diff *= -1;
				}

				if (x_diff == 1) {
					//float estimatedReward = futureState.RValue;
					RValue += LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - RValue);
			
				} else if (x_diff == -1) {
					//float estimatedReward = futureState.LValue;
					LValue += LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - LValue);
			
				} else if (z_diff == 1) {
					//float estimatedReward = futureState.FValue;
					FValue += LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - FValue);
				
				} else if (z_diff == -1) {
					//float estimatedReward = futureState.BValue;
					BValue += LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - BValue);
		
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

		public void AddCache(string cacheName,int x, int y, int z)
		{
	
			nodes.Add (cacheName, new NodeState[x, y, z]);
		}

		public NodeState TryGetNode(string cacheName,int x, int y,int z)
		{
			var cache = nodes [cacheName];
			return cache [x, y, z];
		}

		public NodeState GetNode(string cacheName,int x, int y,int z,Block associatedBlock)
		{
			var cache = nodes [cacheName];
			if (cache [x, y, z] == null)
				cache [x, y, z] = new NodeState (this,x,y,z,cacheName,associatedBlock);

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

