using UnityEngine;
using ImprovedNPC.Wandering;


namespace ImprovedNPC
{
    public class Main : IMod
    {
        private GameObject _go;
		private GameObject _debugger;
        public string Identifier { get; set; }
        
        public void onEnabled()
        {
            _go = new GameObject();
            _go.AddComponent<HelloBehaviour>();
			if (Config.DEBUGGING == true) {
				_debugger = new GameObject ();
				_debugger.AddComponent<QlearningDebugger> ();
			}

        }

        public void onDisabled()
        {
            UnityEngine.Object.Destroy(_go);
			if (Config.DEBUGGING == true) {
				UnityEngine.Object.Destroy (_debugger);
			}
        }

        public string Name
        {
            get { return "Improved NPCs"; }
        }

        public string Description
        {
            get { return "This is a modification to the base wandering behavior or npc using QLearning."; }
        }
    }
}
