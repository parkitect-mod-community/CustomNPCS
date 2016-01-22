using UnityEngine;

namespace HelloMod
{
    class HelloBehaviour : MonoBehaviour
    {

		public readonly static string GUEST_QLEARNING = "GUEST";
		float _nextspawnTime = 0.0f;
		void Start()
		{
			QLearningCache.Instance.AddCache(GUEST_QLEARNING,GameController.Instance.park.xSize, GameController.Instance.park.ySize, GameController.Instance.park.zSize);


			/*EventManager.Instance.OnNewThought += (Person person) => {
				

				int currentX = Mathf.FloorToInt(person.currentBlock.transform.transform.position.x);
				int currentY = Mathf.RoundToInt(person.currentBlock.transform.transform.position.y);
				int currentZ = Mathf.FloorToInt(person.currentBlock.transform.transform.position.z);

				int previousX = Mathf.FloorToInt(person.previousBlock.transform.transform.position.x);
				int previousY = Mathf.RoundToInt(person.previousBlock.transform.transform.position.y);
				int previousZ = Mathf.FloorToInt(person.previousBlock.transform.transform.position.z);

				var node = QLearningCache.Instance.GetNode (HelloBehaviour.GUEST_QLEARNING, currentX, currentY, currentZ);
				var thoughts = person.thoughts;
				switch(thoughts[thoughts.Count-1].emotion)
				{
				case Thought.Emotion.ANGRY:

					node.calculateNewState (previousX, previousY, previousZ, -1);
					break;

				case Thought.Emotion.HAPPY:

					node.calculateNewState (previousX, previousY, previousZ, 1);
					break;

				}
			
			};*/


		}

		void Update()
		{
			Global.PERSON_SPAWN = false;
			if (Input.GetKeyDown(KeyCode.M) || Time.time > _nextspawnTime) {


				Prefabs prefab = Prefabs.Guest;

				var guest = this.spawnCustomPerson(prefab);
				guest.gameObject.AddComponent<BehaviorInjector> ();
			

				if (guest != null)
				{
					if (UnityEngine.Random.Range(0, 100) <= 5)
					{
						guest.uniqueID = ScriptableSingleton<UniquePersonManager>.Instance.getRandomAvailableUniquePersonId();
					}
					guest.Initialize();
				}

				int num = GameController.Instance.park.calculateMaxGuestCount();
				if (WeatherController.Instance.IsRaining)
				{
					num /= 4;
				}

				int num2 = Mathf.Max(1, num - GameController.Instance.park.getGuests().Count);
				float max = 1f / (float)num2 * 60f * 5f;
				float num3 = Utility.expovariate(max);
				num3 = Mathf.Clamp(num3, 2f, 90f);
				this._nextspawnTime = Time.time + num3;
			}
		}

		public Person spawnCustomPerson(Prefabs prefab)
		{

			if (GameController.Instance.park.spawns.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, GameController.Instance.park.spawns.Count - 1);
				Spawn spawn = GameController.Instance.park.spawns[index];
				return ScriptableSingleton<AssetManager>.Instance.instantiatePrefab<Person>(prefab, spawn.transform.position, spawn.transform.rotation);
			}
			return null;
		}
        void OnGUI()
        {
            GUIStyle style = new GUIStyle();

            style.fontSize = 100;

            GUI.Label(new Rect(50, 50, 400, 200), "Hello mods1!", style);
        }
    }
}
