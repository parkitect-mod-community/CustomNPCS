using System;
using System.Collections.Generic;

namespace ImprovedNPC.Wandering
{
	public class WeightedPick<T>
	{
		public class WeightPair<P>
		{
			public WeightPair(P item,float weight)
			{
				this.Weight = weight;
				this.Item = item;
			}
			public float Weight{get;set;}
			public P Item{get;set;}

		}
		private float _lastvalue = 0;
		private float _maxNegatve = 0;
		private float _maxPositive = 0;
		private List<WeightPair<T>> _items = new List<WeightPair<T>>();
		public WeightedPick ()
		{
		}

		public int NumberOfPairs()
		{
			return _items.Count;
		}
		public void Add(float value, T obj)
		{
			if (value < 0) {
				_maxNegatve +=Math.Abs( ((value * .8f)-1));
				_items.Add (new WeightPair<T>(obj,_maxNegatve));
			} else {
				_maxPositive += Math.Abs( value+1);
				_lastvalue = value + 1;
				_items.Add (new WeightPair<T>( obj,_maxPositive ));
			}

		}

		public T RandomPick()
		{
			float value = (UnityEngine.Random.value * (_maxPositive + _maxNegatve +_lastvalue)) - (_maxNegatve+_lastvalue);
			UnityEngine.Debug.Log ("-------------------------------------------------------");
			for (int x = 0; x < _items.Count; x++) {
				UnityEngine.Debug.Log (_items [x].Weight + "<" + value);
			}
			for (int x = 0; x < _items.Count; x++) {
				if (_items [x].Weight < value) {
					return _items [x].Item;
				}
			}

			return default(T);

		}
	}
}

