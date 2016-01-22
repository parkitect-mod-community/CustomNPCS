using System;
using  System.Collections.Generic;
namespace HelloMod
{
	public class QLearningCache
	{
		public class NodeState
		{
			private const float LEARNING_RATE = .9f;
			private const float DISCOUNT_FACTOR = .2f;

			public float value { get; private set; }
			private QLearningCache _cache;
			private string _cacheName;
			public NodeState(QLearningCache cache,string cacheName)
			{
				_cacheName = cacheName;
				_cache = cache;
				value= 0;
			}

			public void calculateNewState(int previousx,int previousy,int previousz,float reward)
			{
				
				float estimatedReward = _cache.GetNode (_cacheName, previousx, previousy, previousz).value;
				float oldValue = value;

				value = oldValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * estimatedReward - oldValue);

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

		public NodeState GetNode(string cacheName,int x, int y,int z)
		{
			var cache = nodes [cacheName];
			if (cache [x, y, z] == null)
				cache [x, y, z] = new NodeState (this,cacheName);

			return cache [x, y, z];

		}

		public QLearningCache.NodeState[,,] GetAllNodes(string cacheName)
		{
			return nodes [cacheName];
		}

	}
}

