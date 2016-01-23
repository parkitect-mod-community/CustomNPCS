using System;
using  System.Collections.Generic;
namespace HelloMod
{
	public class QLearningCache
	{
		public class NodeState
		{
			private const float LEARNING_RATE = .8f;
			private const float DISCOUNT_FACTOR = .6f;

			public float LValue { get; private set; }
			public float FLValue { get; private set; }
			public float BLValue { get; private set; }

			public float RValue { get; private set; }
			public float FRValue {get;private set;}
			public float BRValue { get; private set; }

			public float FValue { get; private set; }
			public float BValue { get; private set; }

			public int X{get;private set;}
			public int Y {get;private set;}
			public int Z {get;private set;}

			private QLearningCache _cache;
			private string _cacheName;
			public NodeState(QLearningCache cache,int x,int y, int z,string cacheName)
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
			}

			public float getValueBasedOnLocation(NodeState futureState)
			{
	
				return findMaxUtility (futureState);

			}

			public float findMaxUtility(NodeState futureState)
			{
				float maxUtility = 0.0f;
				if (futureState == null) {
					return 0.0f;
				}

				if (futureState.FRValue > maxUtility) {
					maxUtility = futureState.FRValue;
				}
				if (futureState.FLValue > maxUtility) {
					maxUtility = futureState.FLValue;
				}
				if (futureState.BRValue > maxUtility) {
					maxUtility = futureState.BRValue;
				}
				if (futureState.BLValue > maxUtility) {
					maxUtility = futureState.BLValue;
				}
				if (futureState.RValue > maxUtility) {
					maxUtility = futureState.RValue;
				}
				if (futureState.LValue > maxUtility) {
					maxUtility = futureState.LValue;
				}
				if (futureState.FValue > maxUtility) {
					maxUtility = futureState.FValue;
				}
				if (futureState.BValue > maxUtility) {
					maxUtility = futureState.BValue;
				}
				return maxUtility;

			}

			public void calculateNewState(NodeState futureState,float reward,bool IsBounce)
			{
				int x_diff = (futureState.X - X);
				int z_diff = (futureState.Z - Z);


				float maxUtility = 0.0f;
				if (!IsBounce) {
					maxUtility = findMaxUtility (futureState);
				} else {
					x_diff *= -1;
					z_diff *= -1;
				}


				if (x_diff == 1 && z_diff == 1) {
				//	float estimatedReward = futureState.FRValue;
					FRValue = FRValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - FRValue);
				
				} else if (x_diff == -1 && z_diff == 1) {
				//	float estimatedReward = futureState.FLValue;
					FLValue = FLValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - FLValue);
				
				} else if (x_diff == -1 && z_diff == -1) {
					//float estimatedReward = futureState.BLValue;
					BLValue = BLValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - BLValue);
				
				} else if (x_diff == 1 && z_diff == -1) {
					//float estimatedReward = futureState.BRValue;
					BRValue = BRValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - BRValue);
			
				} else if (x_diff == 1) {
					//float estimatedReward = futureState.RValue;
					RValue = RValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - RValue);
			
				} else if (x_diff == -1) {
					//float estimatedReward = futureState.LValue;
					LValue = LValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - LValue);
			
				} else if (z_diff == 1) {
					//float estimatedReward = futureState.FValue;
					FValue = FValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - FValue);
				
				} else if (z_diff == -1) {
					//float estimatedReward = futureState.BValue;
					BValue = BValue + LEARNING_RATE * (reward + DISCOUNT_FACTOR * maxUtility - BValue);
		
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

		public NodeState GetNode(string cacheName,int x, int y,int z)
		{
			var cache = nodes [cacheName];
			if (cache [x, y, z] == null)
				cache [x, y, z] = new NodeState (this,x,y,z,cacheName);

			return cache [x, y, z];

		}

		public QLearningCache.NodeState[,,] GetAllNodes(string cacheName)
		{
			return nodes [cacheName];
		}

	}
}

