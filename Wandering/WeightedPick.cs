using System;
using System.Collections.Generic;

namespace ImprovedNPC.Wandering
{
	public class WeightedPick<T>
	{
		public class WeightPair<P>
		{
			public WeightPair(P item,float communativeWeight,float weight)
			{
				this.CommunativeWeight = communativeWeight;
				this.Item = item;
				this.Weight = weight;
			}
			public float CommunativeWeight{get;set;}
			public float Weight{get;set;}
			public P Item{get;set;}

		}

		private float _maxNegatve = 0;
		private float _maxPositive = 0;
		private SortedList<float,WeightPair<T>> _items = new SortedList<float, WeightPair<T>>();

		private Random random;
		public WeightedPick ()
		{
			random = new Random ();
		}

		public int NumberOfPairs()
		{
			return _items.Count;
		}
		public void Add(float value, T obj,float negativeWeight,float positiveWeight)
		{
			if (value >= 0) {
				float exp = (float)Math.Exp (value * positiveWeight);
				_maxPositive += exp;
				_items.Add (_maxPositive, new WeightPair<T> (obj, _maxPositive, value));
	
			} else {
				float exp = (float)Math.Exp(value* negativeWeight);
				_maxNegatve += exp;
				_items.Add (-_maxNegatve, new WeightPair<T> (obj, -_maxPositive, value));
			}

		}

		public WeightPair<T> RandomPick()
		{
			
			if (NumberOfPairs () == 1) {
				foreach (KeyValuePair<float,WeightPair<T>> kvp in _items) {
					return kvp.Value;

				}
			}
			float value = (((float)random.NextDouble() * (_maxPositive+_maxNegatve)))-_maxNegatve;


			/*UnityEngine.Debug.Log ("------------------------------------------------------");
			UnityEngine.Debug.Log (value + " max_pos:" + _maxPositive + " last_value:" + _lastvalue + " lowest value:"+ _lowestValue);
			UnityEngine.Debug.Log ("------------------------------------------------------");
			for (int x = 0; x < _items.Count; x++) {
				UnityEngine.Debug.Log (_items [x].CommunativeWeight);
			}
			UnityEngine.Debug.Log ("------------------------------------------------------");
*/
			UnityEngine.Debug.Log ("------------------------------------------------------");
			UnityEngine.Debug.Log (value + " max_positive:" + _maxPositive + " max_negative:" + _maxNegatve);
			foreach (KeyValuePair<float,WeightPair<T>> kvp in _items) {
				UnityEngine.Debug.Log (kvp.Key);
				
			}
			UnityEngine.Debug.Log ("------------------------------------------------------");

			foreach (KeyValuePair<float,WeightPair<T>> kvp in _items) {
				if (kvp.Key >= 0) {
					if (kvp.Key > value) {
						return kvp.Value;
					}
				} else {
					if (kvp.Key < value) {
						return kvp.Value;
					}
				}
			}



			return null;

		}
			
	}
}

