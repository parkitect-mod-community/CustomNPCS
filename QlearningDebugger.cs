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
			
		}

		void OnGUI ()
		{
			var cache = QLearningCache.Instance.GetAllNodes (HelloBehaviour.GUEST_QLEARNING);
			for (int x = 0; x < GameController.Instance.park.xSize; x++) {
				for (int y = 0; y < GameController.Instance.park.ySize; y++) {
					for (int z = 0; z < GameController.Instance.park.zSize; z++) {
						if (cache [x, y, z] != null) {
							// Find the 2D position of the object using the main camera
							Vector2 boxPosition = Camera.main.WorldToScreenPoint(new Vector3(x+.5f,y+.5f,z+.5f));

							// "Flip" it into screen coordinates
							boxPosition.y = Screen.height - boxPosition.y;

							// Center the label over the coordinates
							boxPosition.x -=100 * 0.5f;
							boxPosition.y -= 100 * 0.5f;

							// Draw the box label
							GUI.Box(new Rect(boxPosition.x, boxPosition.y, 300, 20), 
								" L:" + Math.Truncate( cache [x, y, z].LValue*100) + 
								", FL:" + Math.Truncate( cache [x, y, z].FLValue*100) + 
								", BL:" +  Math.Truncate(cache [x, y, z].BLValue*100) + 
								", R:" +  Math.Truncate(cache [x, y, z].RValue*100) +
								", FR:" +  Math.Truncate(cache [x, y, z].FRValue*100) + 
								", BR:" +  Math.Truncate(cache [x, y, z].BRValue*100) + 
								", F:" +  Math.Truncate(cache [x, y, z].FValue*100) + 
								", B:" +  Math.Truncate(cache [x, y, z].BValue*100) );
						}
					}
				}
			}
		}


	}
}
	