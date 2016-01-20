using UnityEngine;
namespace HelloMod
{
    public class Main : IMod
    {
        private GameObject _go;
        
        public string Identifier { get; set; }
        
        public void onEnabled()
        {
            _go = new GameObject();
            _go.AddComponent<HelloBehaviour>();
        }

        public void onDisabled()
        {
            UnityEngine.Object.Destroy(_go);
        }

        public string Name
        {
            get { return "Hello Mod"; }
        }

        public string Description
        {
            get { return "Validates if mods are working on your PC"; }
        }
    }
}
