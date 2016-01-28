using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace ImprovedNPC.Wandering
{
	public class BehaviorInjector : MonoBehaviour
	{
		private List<PersonBehaviour> _behaviours;
		private Guest _guest;
		void Start()
		{
			this.ExchangeBehavior<RoamingBehaviour,CustomWandering> ();	
          
        }

		public override string ToString ()
		{
			string output = "";
			foreach (var behavior in _behaviours) {
				output += behavior.ToString() + ",";
			}
			return output;
		}
			

		private void GetBehaviours()
		{
			var guest = this.gameObject.GetComponent<Guest> ();
			_guest = guest;

			BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;

			FieldInfo field = guest.GetType ().BaseType.GetField ("behaviours", flags);

			_behaviours = (List<PersonBehaviour>)field.GetValue (guest);
		}

		private T getBehavior<T>() where T :  PersonBehaviour
		{
			GetBehaviours ();
			T output = null;
			for (int x = 0; x < _behaviours.Count; ++x) {
				if (_behaviours[x] is T) {
					output = (T)_behaviours[x];
					break;
				}
			}
			return output;
		}

		private E ExchangeBehavior<T,E>()  
			where T :  PersonBehaviour
			where E : PersonBehaviour
		{
			GetBehaviours ();
			bool sameCurrentBehavior = false;
			for (int x = 0; x < _behaviours.Count; ++x) {
				if (_behaviours[x] is T) {
					if (_guest.currentBehaviour is T)
						sameCurrentBehavior = true;


					UnityEngine.Object.DestroyImmediate (_behaviours [x]);
					_behaviours.RemoveAt (x);
					break;
				}
			}

			E behavior = this.gameObject.AddComponent <E>() as E;
			behavior.enabled = false;
			if (sameCurrentBehavior) {
				_guest.instantlyChangeBehaviour<CustomWandering> ();
			}
			_behaviours.Add (behavior);
			return behavior;
		}
			
	}
}

