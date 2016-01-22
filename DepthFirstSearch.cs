using System;
using UnityEngine;
using System.Collections.Generic;

namespace HelloMod
{
	public class DepthFirstSearch
	{
		public class DepthPathNode : Pathfinding.PathNode
		{
			public int Depth{ get; private set;}
            protected Pathfinding.NodeCache _cache;
            public float CalculatedCost{ get;protected set;} 


            public DepthPathNode(DepthPathNode depthNode,Pathfinding.NodeCache cache)
			{
				//increase the depth for the next down
				Depth = depthNode.Depth + 1;
                parentNode = depthNode;
                _cache = cache;
			}

			public DepthPathNode()
			{
				Depth = 0;
			}

            protected virtual void addToOpenNodes(SortedList<float, DepthPathNode> openNodes,DepthPathNode node, Pathfinding.NodeCache nodeCache,int x, int y, int z)
            {
                node.x = x;
                node.y = y;
                node.z = z;

                float x_distance = (float)Math.Abs (node.parentNode.x - node.x);
                float y_distance = (float)Math.Abs (node.parentNode.y - node.y);
                float z_distance = (float)Math.Abs (node.parentNode.z - node.z);
                float cost = node.calculateCosts (node.parentNode, nodeCache.pathfindingAgent);
                float distance =  (float)Math.Sqrt( x_distance*x_distance + y_distance*y_distance + z_distance*z_distance);

               
                openNodes.Add ( cost + distance, node);
                nodeCache.cacheNode (node);
            }

            public virtual void Expand( Pathfinding.NodeCache nodeCache, SortedList<float, DepthPathNode> openNodes, bool canTransport )
            {
                Block block = GameController.Instance.park.blockData.getBlock(this.x, this.y, this.z);
                if (block == null)
                {
                    return;
                }
              

                BlockNeighbour[] connected = block.getConnected ();
                for (int i = 0; i < connected.Length; ++i) {
                    Pathfinding.PathNode pathNode = nodeCache.getNodeAt(
                        Mathf.FloorToInt(connected[i].block.transform.position.x), 
                        Mathf.RoundToInt(connected[i].block.transform.position.y), 
                        Mathf.FloorToInt(connected[i].block.transform.position.z));

                    //skip the node becasue it's consumed
                    if (pathNode != null) {
                        continue;
                    }

                    if (nodeCache.pathfindingAgent.canUseForPathfinding (connected [i].block)) {
                        if (canTransport && connected [i].block is PathToAttraction) {
                            PathToAttraction pathToAttaction = connected [i].block as PathToAttraction;
                            StationController stationController = pathToAttaction.getStationController ();
                            if (stationController == null) {
                                continue;
                            }
                            Attraction attraction = stationController.getAttraction ();
                            if ((attraction == null) || !nodeCache.pathfindingAgent.canUseForPathfinding (attraction) || !attraction.queueCanBeEntered () || pathToAttaction.getStationController ().getEntrance() == null) {
                                continue;
                            }

                            this.addToOpenNodes (openNodes, new StationDepthNode (this, stationController,_cache),nodeCache, 
                                Mathf.FloorToInt(connected[i].block.transform.position.x),
                                Mathf.RoundToInt(connected[i].block.transform.position.y),
                                Mathf.FloorToInt(connected[i].block.transform.position.z));
                           
                        } else {

                            this.addToOpenNodes (openNodes, new DepthPathNode (this,_cache),nodeCache,
                                Mathf.FloorToInt(connected[i].block.transform.position.x),
                                Mathf.RoundToInt(connected[i].block.transform.position.y),
                                Mathf.FloorToInt(connected[i].block.transform.position.z));

                        }
                    }

                
                }
                
            }

            public override float calculateCosts (Pathfinding.PathNode fromNode, IPathfindingAgent pathfindingAgent)
            {
                float interest = 0;
                float distanceCost = 0;
                if (pathfindingAgent is Guest) {
                    var guest = ((Guest)pathfindingAgent);

                    Block block = GameController.Instance.park.blockData.getBlock(fromNode.x,fromNode.y,fromNode.z);


                    if (guest.previousBlock != null) {
                        if (fromNode.x == ((int)((Guest)pathfindingAgent).previousBlock.intPosition.x) &&
                        fromNode.y == ((int)((Guest)pathfindingAgent).previousBlock.intPosition.y) &&
                        fromNode.z == ((int)((Guest)pathfindingAgent).previousBlock.intPosition.z)) {
                            return 1000f;
                        }
                    }
                    PersonBehaviour[] components = guest.GetComponents<PersonBehaviour>();
                    for (int j = 0; j < components.Length; j++)
                    {
                        PersonBehaviour personBehaviour = components[j];
                        if (personBehaviour is WalkToInterestCalculator)
                        {
                            var num = (personBehaviour as WalkToInterestCalculator).calculateInterest (block, (Person)pathfindingAgent);
                            if (num > interest) {
                                interest = num;
                            }

                        }
                    }

                }
                return  CalculatedCost =   base.calculateCosts (fromNode, pathfindingAgent) + (1.0f/(interest+1.0f));
            }
			
		}


        public interface ISpecialNode{}


        public class StationDepthNode : DepthPathNode, ISpecialNode
        {
            public StationController _controller { get; private set;}
            public bool enter{get;set;}
            public StationDepthNode(DepthPathNode pathNode,StationController controller,Pathfinding.NodeCache cache):base(pathNode,cache)
            {
                _controller = controller;
                enter = true;
                
            }

            public override void Expand (Pathfinding.NodeCache nodeCache, SortedList<float, DepthPathNode> openNodes, bool canTransport)
            {
                if (!enter) {
                    base.Expand (nodeCache, openNodes, canTransport);
                    return;
                }

                foreach (StationController current in _controller.getAttraction().getStationControllers()) {
                    if (current != _controller && !(current.getExit () == null)) {
                        Pathfinding.PathNode pathNode = nodeCache.getNodeAt(Mathf.FloorToInt(current.getExit().transform.position.x), Mathf.RoundToInt(current.getExit().transform.position.y), Mathf.FloorToInt(current.getExit().transform.position.z));
                        if (pathNode == null) {
                            
                            addToOpenNodes (openNodes, new DepthPathNode (this,_cache), nodeCache,
                                Mathf.FloorToInt(current.getExit().transform.position.x),
                                Mathf.RoundToInt(current.getExit().transform.position.y),
                                Mathf.FloorToInt(current.getExit().transform.position.z));
                        }
                    }
                }
            }

            public override float calculateCosts (Pathfinding.PathNode fromNode, IPathfindingAgent pathfindingAgent)
            {
                if (fromNode is Pathfinding.StationPathNode)
                {
                    Pathfinding.StationPathNode stationPathNode = fromNode as Pathfinding.StationPathNode;
                    TrackedRide trackedRide = this._controller.getAttraction() as TrackedRide;
                    float num = trackedRide.stats.averageVelocity * 1f / 3f;
                    int nextSegmentIndex = trackedRide.Track.getNextSegmentIndex((stationPathNode.stationController as TrackedRideStationController).stationEndIndex);
                    float distanceBetween = trackedRide.Track.getDistanceBetween(nextSegmentIndex, (this._controller as TrackedRideStationController).stationEndIndex);
                    float num2 = distanceBetween;
                    if (num > 0f)
                    {
                        num2 /= num;
                    }
                    float num3 = Vector3.Distance(stationPathNode.stationController.getEntrance().transform.position, this._controller.getExit().transform.position);
                    float num4 = trackedRide.entranceFee / num3;
                    return num2 + num4 * 100f;
                }
                return base.calculateCosts(fromNode, pathfindingAgent) + (float)this._controller.getAverageWaitingTime();
            
            }
        }

        private class Comparer : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                if ((int)(x - y) == 0)
                {
                    return 1;
                }
                return (int)(x - y);
            }
        }

		private Pathfinding.NodeCache _nodeCache;
		private DepthPathNode _startNode;

		private int _startX;
		private int _startY;
		private int _startZ;


		public bool CanUseTransport{get;set;}

        public List<Pathfinding.PathNode> path;

        private SortedList<float, DepthPathNode> _possibleNodes = new SortedList<float, DepthPathNode>(new Comparer());
        private SortedList<float, DepthPathNode> _openNodes = new SortedList<float, DepthPathNode>(new Comparer());

		public PathFindingDebugger pathFindingDebugger;

        public DepthFirstSearch (int maxDepth,Vector3 start, IPathfindingAgent pathfindingAgent)
		{
			_nodeCache = new Pathfinding.NodeCache (GameController.Instance.park.xSize, GameController.Instance.park.ySize, GameController.Instance.park.zSize);

			_startX = Mathf.FloorToInt(start.x);
			_startY = Mathf.FloorToInt (start.y);
			_startZ = Mathf.FloorToInt (start.z);

			_startNode = (DepthPathNode)_nodeCache.cacheNode (new DepthPathNode () {
				x = _startX,
				y = _startY,
				z = _startZ
			});

			_startNode.f = 0f;

			_nodeCache.startNode = _startNode;
            _nodeCache.pathfindingAgent = pathfindingAgent;

            _nodeCache.cacheNode (_startNode);


		}

		public void run()
		{
            _startNode.Expand(_nodeCache,_openNodes,true);

			while (_openNodes.Count > 0) {
				if (IsFinished ()) {
					break;
				}
			}
		}

		public bool IsFinished()
		{
            DepthPathNode pathNode = _openNodes.Values[0];

            _possibleNodes.Add (pathNode.CalculatedCost,pathNode);
            _openNodes.RemoveAt(0);
			//pathNode.isOpen = false;
            if (pathNode.Depth <= 5) {
                pathNode.Expand (_nodeCache, _openNodes, true);
            }
            if (_openNodes.Count == 0) {

                this.path = this.reconstructPath(_possibleNodes.Values[0], _startNode, new List<Pathfinding.PathNode>());
                return true;
            }
			return false;
		}

        private List<Pathfinding.PathNode> reconstructPath(Pathfinding.PathNode pathNode, Pathfinding.PathNode startNode, List<Pathfinding.PathNode> path)
        {
            path.Add(pathNode);
            if (!pathNode.Equals(startNode))
            {
                this.reconstructPath(pathNode.parentNode, startNode, path);
            }
            return path;
        }

	}
}

