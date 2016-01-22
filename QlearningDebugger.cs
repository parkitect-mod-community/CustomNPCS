using System;
using UnityEngine;

namespace HelloMod
{
	public class QlearningDebugger : MonoBehaviour
	{
		public QlearningDebugger ()
		{
		}

		private void Update()
		{
			var cache = QLearningCache.Instance.GetAllNodes (HelloBehaviour.GUEST_QLEARNING);
			for (int x = 0; x < GameController.Instance.park.xSize; x++) {
				for (int y = 0; y < GameController.Instance.park.ySize; y++) {
					for (int z = 0; z < GameController.Instance.park.zSize; z++) {
						if (cache [x, y, z] != null) {
							UnityEngine.Debug.DrawLine (new UnityEngine.Vector3 ((float)x + .5f,(float) y + .5f, (float)z + .5f), new UnityEngine.Vector3 ((float)x + .5f, (float)y + .5f, (float)z + (.5f * cache [x, y, z].value)));
						}
					}
				}
			}
		}
	}
}

