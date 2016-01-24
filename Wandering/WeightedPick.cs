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
				_maxNegatve += (Math.Abs(value) * .2f)+1;
				_items.Add (new WeightPair<T>(obj,-_maxNegatve,value));
			} else {
				_maxPositive += Math.Abs( value)+1;
				_lastvalue = value + 1;
				_items.Add (new WeightPair<T>( obj,_maxPositive,value));
			}

		}

		public WeightPair<T> RandomPick()
		{
			if (NumberOfPairs () == 1) {
				return _items [0];
			}
			
			float value = (UnityEngine.Random.value * (_maxPositive + _maxNegatve +_lastvalue)) - _maxNegatve;
			for (int x = 0; x < _items.Count; x++) {
				if (_items [x].CommunativeWeight < value) {
					return _items [x];
				}
			}

			return null;

		}
	}
}

