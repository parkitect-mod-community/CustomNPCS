using System;
using UnityEngine;

namespace HelloMod
{
	public class CustomGuest : Guest
	{
		public CustomGuest ()
		{
			Start ();
			Initialize ();
		}
		public override void Initialize ()
		{
			base.Initialize ();
		}

		public override void Start ()
		{
			UnityEngine.Debug.Log ("test");
			base.Start ();
		}
		public override void Update ()
		{


			UnityEngine.Debug.Log (this.getDebugInfo());
			//base.Update ();
		
		}
	}
}

