using System;
using UnityEngine;

namespace HelloMod
{
	public class QLearning : MonoBehaviour
	{
		private Block _currentBlock = null;
		private Guest _guest;
		public QLearning ()
		{

		}

		void Start()
		{
			_guest = GetComponent<Guest> ();
		}

		void Update()
		{
			if (_guest.currentBlock != _currentBlock) {
				Block previousBlock = _currentBlock;
				_currentBlock = _guest.currentBlock;
				if(previousBlock != null)
				{
					int currentX = Mathf.FloorToInt(_currentBlock.transform.transform.position.x);
					int currentY = Mathf.RoundToInt(_currentBlock.transform.transform.position.y);
					int currentZ = Mathf.FloorToInt(_currentBlock.transform.transform.position.z);

					int previousX = Mathf.FloorToInt(previousBlock.transform.transform.position.x);
					int previousY = Mathf.RoundToInt(previousBlock.transform.transform.position.y);
					int previousZ = Mathf.FloorToInt(previousBlock.transform.transform.position.z);

					var node = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, currentX, currentY, currentZ);
					node.calculateNewState (previousX, previousY, previousZ, 0);
				}
			}
	

		}
	}
}

