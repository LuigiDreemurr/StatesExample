using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : FSMState
{

    const int ATTACK_DIST = 2;
    const int CHASE_DIST = 20;
    const int LOST_DIST = 45;
    bool moveCloser;

    //Constructor
    public AttackState(Transform[] wp)
    {

        waypoints = wp;
        stateID = FSMStateID.Attacking;
        curSpeed = 4.0f;
        curRotSpeed = 2.0f;
        moveCloser = false ;

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
        if (npc.GetComponent<AIController>().GetHealth() < 50)
        {
            npc.GetComponent<AIController>().PerformTransition(Transition.LowHealth);
            return;
        }

        //Chech player distance
        if (IsInCurrentRange(npc, destPos, ATTACK_DIST))
        {
            moveCloser = false;
        }
        else if (!IsInCurrentRange(npc, destPos, LOST_DIST))
        {
            npc.GetComponent<AIController>().PerformTransition(Transition.LostPlayer);
            return;
        }
        else
        {
            moveCloser = true;
        }

    }


    public override void Act(Transform player, Transform npc)
    {

        destPos = player.position;
        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        if(moveCloser)
            npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
        if (Mathf.Pow(Mathf.Pow(npc.position.x - player.transform.position.x, 2) + Mathf.Pow(npc.position.y - player.transform.position.y, 2), 0.5f) <= 7)
        {
            player.GetComponent<PCControl>().health -= 25 * Time.deltaTime;
            //Debug.Log("Hit");
        }

    }

}
