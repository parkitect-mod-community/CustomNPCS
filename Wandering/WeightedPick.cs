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
		private List<WeightPair<T>> _items = new List<WeightPair<T>>();

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
				_items.Add ( new WeightPair<T> (obj, _maxPositive, exp));
	
			} else {
				float exp = (float)Math.Exp(value* negativeWeight);
				_maxNegatve += exp;
				_items.Add ( new WeightPair<T> (obj, -_maxNegatve, -exp));
			}

		}

		public WeightPair<T> RandomPick()
		{
			if (NumberOfPairs () == 1) {
				
				return _items [0];
			}
			float value = (((float)random.NextDouble() * (_maxPositive+_maxNegatve)))-_maxNegatve;


			foreach (var item in _items) 
			{
				if (item.CommunativeWeight > 0 && value > 0) {
					if (value < item.CommunativeWeight ) {
						return item;
					}
				} 
				else if(item.CommunativeWeight < 0 && value < 0)
				{
					if (value > item.CommunativeWeight) {
						return item;
					}
				}

			}



			return null;

		}
			
	}
}

