using UnityEngine;
using System.Collections;

public class DeadState : FSMState
{

    private bool exploded;

	//Constructor
    public DeadState(Transform[] wp)
	{

		waypoints = wp;
		stateID = FSMStateID.Dead;
		curSpeed = 0.0f;
		curRotSpeed = 0.0f;
        exploded = false;


    }

	//Reason
	public override void Reason( Transform player, Transform npc)
	{
		
        //do nothing

	}
	//Act
	public override void Act( Transform player, Transform npc)
	{
        
        //do nothing
        //or just play dead forever... Im sure they'll buy it!
        //if (!exploded) not working for some reason...
        //{
        //    Instantiate(npc.GetComponent<AIController>().explosion, npc.position);
        //}

	}

}
