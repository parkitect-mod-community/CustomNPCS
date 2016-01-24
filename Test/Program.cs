using System;
using ImprovedNPC.Wandering;

namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Random random = new Random ();
			while (true) {
				WeightedPick<int> p = new WeightedPick<int> ();
				p.Add ((float)random.NextDouble()*.00004f, 1, .4f, .004f);
				p.Add (-(float)random.NextDouble()*.00004f, 2, .4f, .004f);

				Console.WriteLine ("picked:"+p.RandomPick ().Item);
			}
		}
	}
}
