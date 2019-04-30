using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIController : AdvancedFSM
{

    public bool debugDraw;
    public Text StateText;
    public bool AI1;
    public bool AI2;
    public GameObject explosion;
    private bool exploded;

    //Set/Get/Decrement/Add to Health functions
    public float health;
    public int MaxHealth;
    public float GetHealth() { return health; }
    public void SetHealth(int inHealth) { health = inHealth; }
    public void DecHealth(int amount) { Mathf.Max(0, health - amount); }
    public void AddHealth(int amount) { Mathf.Min(100, health + amount); }

    private string GetStateString()
    {
        string state = "NONE";
        
        if (CurrentState.ID == FSMStateID.Dead)
        {
            state = "DEAD";
            if (!exploded)
            {
                //Instantiate(explosion, position); this should be working... WHY DOESN'T IT WORK?!!!!
                exploded = true;
            }
        }
        else if (CurrentState.ID == FSMStateID.Attacking)
        {
            state = "ATTACK";
        }
        else if (CurrentState.ID == FSMStateID.Fleeing)
        {
            state = "FLEE";
        }
        else if (CurrentState.ID == FSMStateID.Hiding)
        {
            state = "HIDE";
        }
        else if (CurrentState.ID == FSMStateID.Patrolling)
        {
            state = "Patrol";
        }


        return state;
    }
    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        health = 100;
        ConstructFSM();
        exploded = false;
    }


    protected override void FSMUpdate()
    {

        elapsedTime += Time.deltaTime;
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);
        StateText.text = "AI STATE IS: " + GetStateString();
        if (debugDraw)
        {
            UsefullFunctions.DebugRay(transform.position, transform.forward * 5.0f, Color.red);
        }

    }


    private void ConstructFSM()
    {
        pointList = GameObject.FindGameObjectsWithTag("WayPoint");
        //
        //Creating a wapoint transform array for each state
        //
        Transform[] waypoints = new Transform[pointList.Length];
        int i = 0;
        foreach (GameObject obj in pointList)
        {
            waypoints[i] = obj.transform;
            i++;
        }
        //
        //Create States
        //
        HideState hide = new HideState(waypoints);
        AttackState attack = new AttackState(waypoints);


        //Create the Dead state
        DeadState dead = new DeadState(waypoints);
        //there are no transitions out of the dead state

        PatrolState patrol = new PatrolState(waypoints);
        FleeState flee = new FleeState(waypoints);

        //add transitions OUT of the attack state
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        if (AI1)
        {

            attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
            attack.AddTransition(Transition.LowHealth, FSMStateID.Fleeing);

        }
            
        if (AI2)
        {

            attack.AddTransition(Transition.LostPlayer, FSMStateID.Hiding);

        }
            
        attack.AddTransition(Transition.LowHealth, FSMStateID.Fleeing);

        //add transitions OUT of the hide state
        hide.AddTransition(Transition.SawPlayer, FSMStateID.Attacking);
        hide.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //add transitions OUT of the patrol state
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking);
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //add transitions OUT of the flee state
        if (AI1)
            flee.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        if (AI2)
            flee.AddTransition(Transition.LostPlayer, FSMStateID.Hiding);
        flee.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //Add state to the state list
        if (AI1)
            AddFSMState(patrol);
        
        if (AI2)
            AddFSMState(hide);

        AddFSMState(attack);
        AddFSMState(dead);
        
    }

}
