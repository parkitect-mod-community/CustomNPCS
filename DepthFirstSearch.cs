using System;
using UnityEngine;
using System.Collections.Generic;

namespace HelloMod
{
	public class DepthFirstSearch
	{
		public class DepthPathNode : Pathfinding.PathNode
		{
			public int _depth{ get; private set;}
			public DepthPathNode(DepthPathNode depthNode)
			{
				//increase the depth for the next down
				_depth = depthNode._depth + 1;
			}

			public Pathfinding.PathNode ParentNode{ get; set; }

			public DepthPathNode()
			{
				_depth = 0;
			}

			public override void fillReachableNodesList (List<Pathfinding.PathNode> reachableNodes, Pathfinding.NodeCache nodeCache, bool canUseTransportRides)
			{
				Block block = GameController.Instance.park.blockData.getBlock(this.x, this.y, this.z);
				if (block == null) {
					return;
				}
				BlockNeighbour[] connected = block.getConnected();
				for (int x = 0; x < connected.Length; ++x) {
					BlockNeighbour blockNeighbour = connected[i];
					Pathfinding.PathNode pathNode = nodeCache.getNodeAt(

				}

			}

			
		}

		private int _maxDepth;
		private Pathfinding.NodeCache _nodeCache;
		private DepthPathNode _startNode;

		private int _startX;
		private int _startY;
		private int _startZ;

		public bool CanUseTransport{get;set;}

		private SortedList<float, Pathfinding.PathNode> _openNodes = new SortedList<float, Pathfinding.PathNode>(new Pathfinding.Comparer());

		public PathFindingDebugger pathFindingDebugger;
		private List<Pathfinding.PathNode> _reachableNodes = new List<Pathfinding.PathNode>();
		private List<Pathfinding.PathNode> _closeNodes = new List<Pathfinding.PathNode>();

		public DepthFirstSearch (int maxDepth,Vector3 start)
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

		}

		public void run()
		{
			while (_openNodes.Count > 0) {
				if (IsFinished ()) {
					break;
				}
			}
			if (this.pathFindingDebugger != null)
			{
				this.pathFindingDebugger.openNodes = this.openNodes;
				this.pathFindingDebugger.closedNodes = this.closedNodes;
			}
		}

		public bool IsFinished()
		{
			Pathfinding.PathNode pathNode = _openNodes.Values[0];
			_openNodes.RemoveAt(0);
			pathNode.isOpen = false;
			//if (pathNode.isEqualTo(this.targetNode))
			//{
			//	this.path = this.reconstructPath(pathNode, this.startNode, new List<Pathfinding.PathNode>());
			//	return true;
			//}
			this.expandNode(pathNode, _openNodes, _closeNodes);

			return false;
		}

		private void expandNode(Pathfinding.PathNode expandedNode, Pathfinding.PathNode targetNode, SortedList<float, Pathfinding.PathNode> openNodes, List<Pathfinding.PathNode> closedNodes)
		{
			expandedNode.expanded = true;
			closedNodes.Add(expandedNode);
			this._reachableNodes.Clear();
			expandedNode.fillReachableNodesList(this._reachableNodes, this._nodeCache, CanUseTransport);
			for (int i = 0; i < this._reachableNodes.Count; i++)
			{
				Pathfinding.PathNode pathNode = this._reachableNodes[i];
				_nodeCache.cacheNode(pathNode);
				float num = (float)Mathf.Abs(pathNode.x - targetNode.x);
				float num2 = (float)Mathf.Abs(pathNode.y - targetNode.y);
				float num3 = (float)Mathf.Abs(pathNode.z - targetNode.z);
				if (!pathNode.expanded)
				{
					float num4 = pathNode.calculateCosts(expandedNode,_nodeCache.pathfindingAgent) + pathNode.costs + expandedNode.g;
					float num5 = Mathf.Sqrt(num * num + num2 * num2 + num3 * num3);
					float num6 = num4 + num5;
					bool isOpen = pathNode.isOpen;
					if (!isOpen || num6 <= pathNode.f)
					{
						pathNode.parentNode = expandedNode;
						pathNode.g = num4;
						pathNode.f = num6;
						if (!isOpen)
						{
							pathNode.isOpen = true;
							openNodes.Add(pathNode.f, pathNode);
						}
					}
				}
			}
		}
	}
}

