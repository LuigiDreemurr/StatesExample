using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : FSMState
{

    const int CHASE_DIST = 20;
    const int HIDE_WAYPOINT_DIST = 1;
    int currentPoint;      //Randomized hiding points, wants to hide at ?

    //Constructor
    public PatrolState(Transform[] wp)
    {

        waypoints = wp;
        stateID = FSMStateID.Patrolling;
        curSpeed = 3.0f;
        curRotSpeed = 2.0f;
        destPos = waypoints[currentPoint].position;
        currentPoint = UnityEngine.Random.Range(0, 24);

    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {

        //Check if NPC has died, if it has go to the death state (or perform “NoHealth” //transition which is mapped to the death state)
        if (npc.GetComponent<AIController>().GetHealth() == 0)
        {
            npc.GetComponent<AIController>().PerformTransition(Transition.NoHealth);
            return;
        }

        //NOTE: ORDER OF OF STATEMENTS IS IMPORTANT
        //If we are in chasing distance of the player, then go to ATTACK state
        if (IsInCurrentRange(npc, player.position, CHASE_DIST))
        {
            //we saw the player use "SawPlayer" transition (this will take us to the //Attack state)
            destPos = player.position;
            npc.GetComponent<AIController>().PerformTransition(Transition.SawPlayer);
        }
        //check to see if we reached our point
        else if (IsInCurrentRange(npc, destPos, HIDE_WAYPOINT_DIST))
        {

            //go to next point
            currentPoint++;
            if (currentPoint >= 25)
                currentPoint = 0;
            destPos = waypoints[currentPoint].position;
            Debug.Log("Point reached, next is " + currentPoint);

        }

    }

    //Act
    public override void Act(Transform player, Transform npc)
    {

        //rotate towards the target destination position and move towards the destination position
        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);

        if (npc.GetComponent<AIController>().health < npc.GetComponent<AIController>().MaxHealth)
        {
            npc.GetComponent<AIController>().health += 10 * Time.deltaTime;
            if (npc.GetComponent<AIController>().health > npc.GetComponent<AIController>().MaxHealth)
                npc.GetComponent<AIController>().health = npc.GetComponent<AIController>().MaxHealth;
        }

    }

}
