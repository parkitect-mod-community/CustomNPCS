using System;

namespace ImprovedNPC
{
	public static class Config
	{

		//------------------------------------------------------------------------------------- 
		//LEARNING
		//-------------------------------------------------------------------------------------

		//don't set the learning rate or discount factor above 1 or the Q values will diverge.
		public static readonly float LEARNING_RATE = .80f;
		public static readonly float DISCOUNT_FACTOR = .1f;


		//------------------------------------------------------------------------------------- 
		//REWARDS
		//-------------------------------------------------------------------------------------

		//the cost to moving to a new tile. 
		public static readonly float MOVMENT_COST = -.04f;
		public static readonly float BOUNCE_COST = -.1f;

		//------------------------------------------------------------------------------------- 
		//PICK FACTORS
		//-------------------------------------------------------------------------------------

		//a low weighted factor means that an npc is less likly to roll that decision based on Q values of the tile
		//a very high weight factor means that the npc is more likly to roll for that tile

		//the weighted factore used to decide if a NPC will go on a ride. 
		public static readonly float FINDING_ATTRACTION_POSITIVE_WEIGHT_FACTOR = .1f;
		public static readonly float FINDING_ATTRACTION_NEGATIVE_WEIGHT_FACTOR = .1f;

		//the weighted factore used to decide if a NPC move to a tile in a general wandering state.
		public static readonly float WANDERING_POSITIVE_WEIGHT_FACTOR = .1f;
		public static readonly float WANDERING_NEGATIVE_WEIGHT_FACTOR = .1f;



	}
}

