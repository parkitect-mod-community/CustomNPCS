using System;
using UnityEngine;

namespace ImprovedNPC.Wandering
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
			var cache = QLearningCache.Instance.GetAllNodes (ImprovedNPC.GUEST_QLEARNING);
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
							string output = "";
							if(cache [x, y, z].LValue != 0)
								output += " L:" +  cache[x, y, z].LValue;
							if(cache [x, y, z].RValue != 0)
								output += ", R:" + cache [x, y, z].RValue;
							if(cache [x, y, z].FValue != 0)
								output += ", F:" + cache [x, y, z].FValue;
							if(cache [x, y, z].BValue != 0)
								output += ", B:" + cache [x, y, z].BValue;
							GUI.TextArea (new Rect (boxPosition.x, boxPosition.y, 300, 20), output);
						//	GUI.Box(new Rect(boxPosition.x, boxPosition.y, 300, 20), output);
						}
					}
				}
			}
		}


	}
}
	