using System;
using System.Collections.Generic;

namespace ImprovedNPC.Wandering
{
	public class WeightedPick<T>
	{
		private float _maxNegatve = float.MaxValue;
		private float _maxPositive = float.MaxValue;
		private List<Tuple<float,T>> items = new List<Tuple<float, T>>();
		public WeightedPick ()
		{
		}
		public void Add(float value, T obj)
		{
			if (value < 0) {
				_maxNegatve +=Math.Abs( value * .4f);
				items.Add (new Tuple<float, T> (value - _maxNegatve -1, obj));
			} else {
				_maxPositive += Math.Abs( value);
				items.Add (new Tuple<float, T> (value + _maxPositive + 1, obj));
			}

		}

		public T RandomPick()
		{
			float value = (UnityEngine.Random.value * (_maxPositive + _maxNegatve)) - _maxNegatve;

			for (int x = 0; x < items.Count; x++) {
				if (items [x].Item1 < value) {
					return items [x].Item2;
				}
			}
			return items [0].Item2;

			
		}
	}
}

