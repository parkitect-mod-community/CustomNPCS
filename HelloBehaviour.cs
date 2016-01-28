using UnityEngine;
using ImprovedNPC.Wandering;

namespace ImprovedNPC
{
    class HelloBehaviour : MonoBehaviour
    {
     	public readonly static string GUEST_QLEARNING = "GUEST";
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
            QLearningCache.Instance.ClearCache ();
        }

		void Update()
		{
		
		}

       
    }
}
