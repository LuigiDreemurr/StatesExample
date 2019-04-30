using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : FSMState
{

    const int CHASE_DIST = 20;

    //Constructor
    public FleeState(Transform[] wp)
    {

        waypoints = wp;
        stateID = FSMStateID.Fleeing;
        curSpeed = 5.0f;
        curRotSpeed = 3.0f;

    }

    public override void Reason(Transform player, Transform npc)
    {

        destPos = player.position;

        //check if dead
        if (npc.GetComponent<AIController>().GetHealth() == 0)
        {
            npc.GetComponent<AIController>().PerformTransition(Transition.NoHealth);
            return;
        }

        //check low health
        if (npc.GetComponent<AIController>().GetHealth() > 60)
        {
            npc.GetComponent<AIController>().PerformTransition(Transition.LostPlayer);
            return;
        }

        //Chech player distance
        if (!IsInCurrentRange(npc, destPos, CHASE_DIST))
        {
            npc.GetComponent<AIController>().PerformTransition(Transition.LostPlayer);
            return;
        }
        
    }


    public override void Act(Transform player, Transform npc)
    {

        destPos = player.position;
        Quaternion targetRotation = Quaternion.LookRotation(destPos + npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);

    }
}
