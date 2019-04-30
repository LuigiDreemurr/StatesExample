using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : FSMState
{

    const int ATTACK_DIST = 10;
    const int CHASE_DIST = 20;
    const int HIDE_WAYPOINT_DIST = 1;
    int HIDE_WAYPOINT_INDEX = UnityEngine.Random.Range(0, 24);      //Randomized hiding points, wants to hide at ?
    bool moving;

    //Constructor
    public HideState(Transform[] wp)
    {

        waypoints = wp;
        stateID = FSMStateID.Hiding;
        curSpeed = 3.0f;
        curRotSpeed = 2.0f;

        moving = true;
        destPos = waypoints[HIDE_WAYPOINT_INDEX].position;

    }

    public override void Reason(Transform player, Transform npc)
    {

        moving = true;
        destPos = waypoints[HIDE_WAYPOINT_INDEX].position;

        //check if dead
        if(npc.GetComponent<AIController>().GetHealth() == 0)
        {
            npc.GetComponent<AIController>().PerformTransition(Transition.NoHealth);
            return;
        }

       //is player in range
       if(IsInCurrentRange(npc, player.position, CHASE_DIST))
        {
            destPos = player.position;
            npc.GetComponent<AIController>().PerformTransition(Transition.SawPlayer);
        }
       else if(IsInCurrentRange(npc, destPos, HIDE_WAYPOINT_DIST))
        {
            moving = false;
        }

    }


    public override void Act(Transform player, Transform npc)
    {

        if(moving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
            npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
        }
        else
        {

            for (float i = 0; i < 1; i += Time.deltaTime)
            {

            }
            if (npc.GetComponent<AIController>().health < npc.GetComponent<AIController>().MaxHealth)
            {
                npc.GetComponent<AIController>().health += 20 * Time.deltaTime;
                if (npc.GetComponent<AIController>().health > npc.GetComponent<AIController>().MaxHealth)
                    npc.GetComponent<AIController>().health = npc.GetComponent<AIController>().MaxHealth;
            }
            
        }



    }

   

}
