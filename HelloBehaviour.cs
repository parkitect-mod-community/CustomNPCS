using UnityEngine;

namespace HelloMod
{
    class HelloBehaviour : MonoBehaviour
    {

		void Update()
		{
			Global.PERSON_SPAWN = false;
			if (Input.GetKeyDown(KeyCode.M)){//&&Time.time > _nextSpawnTime) {


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

			/*	int num = GameController.Instance.park.calculateMaxGuestCount();
				if (WeatherController.Instance.IsRaining)
				{
					num /= 4;
				}

				int num2 = Mathf.Max(1, num - GameController.Instance.park.getGuests().Count);
				float max = 1f / (float)num2 * 60f * 5f;
				float num3 = Utility.expovariate(max);
				num3 = Mathf.Clamp(num3, 2f, 90f);
				this._nextSpawnTime = Time.time + num3;*/
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
