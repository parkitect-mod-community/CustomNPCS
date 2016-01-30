using UnityEngine;
using ImprovedNPC.Wandering;


namespace ImprovedNPC
{
    public class Main : IMod
    {
        private GameObject _go;
		private GameObject _debugger;
		private GameObject _qLearningCache;
		public string Identifier { get; set; }
        
        public void onEnabled()
        {
			_qLearningCache = new GameObject ();
            _go = new GameObject();
            _go.AddComponent<ImprovedNPC>();
			QLearningCache.Instance = _qLearningCache.AddComponent<QLearningCache> ();
			if (Config.DEBUGGING == true) {
				_debugger = new GameObject ();
				_debugger.AddComponent<QlearningDebugger> ();
			}

        }

        public void onDisabled()
        {
            UnityEngine.Object.Destroy(_go);
			Object.Destroy (_qLearningCache);
			QLearningCache.Instance.ClearCache ();

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
