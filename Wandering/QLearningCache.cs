using  System.Collections.Generic;
using UnityEngine;

namespace ImprovedNPC.Wandering
{
	public class QLearningCache : SerializedMonoBehaviour
	{
		public class NodeState : SerializedRawObject
		{

			public float LValue { get; set; }
			public float RValue { get; set; }
			public float FValue { get; set; }
			public float BValue { get; set; }

			public int X{get;private set;}
			public int Y {get;private set;}
			public int Z {get;private set;}
			public Block AssocaitedBlock{ get; private set; }

			private QLearningCache _cache;
			private string _cacheName;
			public NodeState()
			{
			}
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
				AssocaitedBlock = associatedBlock;
				associatedBlock.OnDestroyed += OnDestroy;
			}

			public void OnDestroy()
			{
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

			}
				
			public float findMaxUtility(NodeState futureState)
			{
				float maxUtility = float.MinValue;
		

				if (futureState.RValue != 0 && futureState.RValue > maxUtility  /*&& x_diff != -1*/) {
					maxUtility = futureState.RValue;
				}
				if (futureState.LValue != 0 && futureState.LValue > maxUtility /*&& x_diff != 1*/) {
						maxUtility = futureState.LValue;
				}
				if (futureState.FValue != 0 &&futureState.FValue > maxUtility /*&& z_diff != -1*/  ) {
						maxUtility = futureState.FValue;
				}
				if (futureState.BValue != 0 &&futureState.BValue > maxUtility/*  && z_diff != 1 */) {
						maxUtility = futureState.BValue;
				}

				if (maxUtility == float.MinValue) {
					maxUtility = float.Epsilon;
				}

				return maxUtility;

			}

			public void calculateNewState(NodeState futureState,float reward,bool bounce)
			{
				int x_diff = (futureState.X - X);
				int z_diff = (futureState.Z - Z);
				if (bounce) {
					z_diff *= -1;
					x_diff *= -1;
				}


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



			public override void serialize (SerializationContext context, Dictionary<string, object> values)
			{
				values.Add ("RValue", RValue);
				values.Add ("LValue", LValue);
				values.Add ("FValue", FValue);
				values.Add ("BValue", BValue);

				values.Add ("x", X);
				values.Add ("y", Y);
				values.Add ("z", Z);
				base.serialize (context, values);
			}
			public override void deserialize (SerializationContext context, Dictionary<string, object> values)
			{
				this.X =  (int)values ["x"];
				this.Y =  (int)values ["y"];
				this.Z =  (int)values ["z"];

				this.RValue = (float)values ["RValue"];
				this.FValue = (float)values ["FValue"];
				this.LValue = (float)values ["LValue"];
				this.BValue = (float)values ["BValue"];


				this.AssocaitedBlock = GameController.Instance.park.blockData.getBlock (X, Y, Z);
				this.AssocaitedBlock.OnDestroyed += OnDestroy;

				base.deserialize (context, values);
			}

			public override string getReferenceName ()
			{
				return "ImprovedNPCDefaultNode";
			}

		}



		private Dictionary<string, QLearningCache.NodeState[,,]> nodes = new Dictionary<string,  QLearningCache.NodeState[,,]>() ;
		private Dictionary<string,IntVector3> cacheSize = new Dictionary<string, IntVector3> ();
		public static QLearningCache Instance;
		
		public QLearningCache ()
		{
            
			
		}

		public bool IsCacheAvailable(string cache)
		{
			return nodes.ContainsKey (cache);
		}

        public void ClearCache()
        {
            nodes.Clear ();
			cacheSize.Clear ();
        }

		public void AddCache(string cacheName,int x, int y, int z)
		{
			cacheSize.Add (cacheName, new IntVector3 (x, y, z));
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
				for (int i = 0; i < connected.Length; i++) {
					int xPos = (int)connected [i].block.intPosition.x;
					int yPos = (int)connected [i].block.intPosition.y;
					int zPos = (int)connected [i].block.intPosition.z;
					if (cache [xPos, yPos, zPos] != null) {
						UpdateBlock (cacheName, xPos, yPos, zPos, cache [xPos, yPos, zPos].AssocaitedBlock.getConnected (), agent);
					}

				}
				UpdateBlock (cacheName, x, y, z, connected, agent);
			}

			return cache [x, y, z];
		}

		private void UpdateBlock(string cacheName,int x, int y, int z, BlockNeighbour[] connected,IPathfindingAgent agent)
		{
			var cache = nodes [cacheName];
			cache [x, y, z].LValue = 0;
			cache [x, y, z].RValue = 0;
			cache [x, y, z].BValue = 0;
			cache [x, y, z].FValue = 0;

			for (int i = 0; i < connected.Length; i++)
			{
				var connect_blocks = connected [i].block;
				if (agent.canUseForPathfinding (connect_blocks)) {

					int xPosition = ((int)connect_blocks.intPosition.x);
					int zPosition = ((int)connect_blocks.intPosition.z);

					int x_diff = xPosition- x;
					int z_diff = zPosition - z;
					if (x_diff == 1 ) {
						//float estimatedReward = futureState.RValue;
						cache [x, y, z].RValue =.00001f;

					} else if (x_diff == -1) {
						//float estimatedReward = futureState.LValue;
						cache [x, y, z].LValue =.00001f;
					} else if (z_diff == 1) {
						//float estimatedReward = futureState.FValue;
						cache [x, y, z].FValue = .00001f;

					} else if (z_diff == -1) {
						//float estimatedReward = futureState.BValue;
						cache [x, y, z].BValue = .00001f;
					}
				}
			}
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

		public override void deserialize (SerializationContext context, Dictionary<string, object> values)
		{
			
			foreach (KeyValuePair<string,Dictionary<string, object>> qmaps in ((Dictionary<string, Dictionary<string, object>>)values["qmaps"])) {

				this.AddCache (qmaps.Key, (int)qmaps.Value ["x"], (int)qmaps.Value ["y"], (int)qmaps.Value ["z"]);

				UnityEngine.Debug.Log ("loading qmap" + qmaps.Key);
				List<Dictionary<string, object>> maps = (List<Dictionary<string, object>>)qmaps.Value ["map"];
				foreach (Dictionary<string, object> map in maps) {
					NodeState state = new NodeState ();
					Serializer.deserialize (context, state, map);
					nodes [qmaps.Key] [state.X, state.Y, state.Z] = state;
				}
			}
			base.deserialize (context, values);
		}

		public override void serialize (SerializationContext context, Dictionary<string, object> values)
		{

			Dictionary<string,object> maps = new Dictionary<string, object> ();
			foreach (KeyValuePair<string,QLearningCache.NodeState[,,]> state in nodes) {
				Dictionary<string, object> container = new Dictionary<string, object> ();

				List<Dictionary<string, object>> nodeState = new List<Dictionary<string, object>> ();

				for (int x = 0; x < cacheSize[state.Key].x; x++) {
					for (int y = 0; y < cacheSize[state.Key].y; y++) {
						for (int z = 0; z < cacheSize[state.Key].z; z++) {
							if (nodes [state.Key] [x, y, z] != null)
								nodeState.Add (Serializer.serialize (context, nodes [state.Key] [x, y, z]));
						}
					}
				}

				container.Add ("x", cacheSize [state.Key].x);
				container.Add ("y", cacheSize [state.Key].x);
				container.Add ("z", cacheSize [state.Key].x);
				container.Add ("map", nodeState);
				maps.Add (state.Key, container);
			}
			values.Add ("qmaps", maps);
			base.serialize (context, values);
		}
			
		public override string getReferenceName ()
		{
			return "ImprovedNPCQLearningCache";
		}
	}
}

