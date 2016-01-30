using UnityEngine;
using ImprovedNPC.Wandering;

namespace ImprovedNPC
{
    class ImprovedNPC : MonoBehaviour
    {
     	public readonly static string GUEST_QLEARNING = "guest_qmap";
    	void Start()
		{
			QLearningCache.Instance.AddCache(GUEST_QLEARNING,GameController.Instance.park.xSize, GameController.Instance.park.ySize, GameController.Instance.park.zSize);

			EventManager.Instance.OnStartPlayingPark += () => {
			    
                Guest[] guests = UnityEngine.Object.FindObjectsOfType(typeof(Guest)) as Guest[];
				foreach(Guest g in guests)
				{
					g.gameObject.AddComponent<BehaviorInjector> ();
				}
                
            };
            EventManager.Instance.OnGuestAdded += (Guest guest) => {
                guest.gameObject.AddComponent<BehaviorInjector> ();

            };

		}

        void OnDestroy()
        {
       	}

		void Update()
		{
		
		}

       
    }
}
