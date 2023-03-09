using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    Transform fellowai;

    EnemyAI enemyAI;

    public LayerMask whatIsGround, whatIsPlayer;

    Material material;

    public Vector3 walkPoint;

    public Transform[] patrolPoints;
    private int whichPatrolPoint = 0;
    public Vector3 mainPatrolPoint;
    public Vector3 playerSeenArea;
    bool walkPointSet;
    //public float walkPointRange;

    public bool searching;
    public bool retreating;
    public bool chasing;

    public float attackedRange, sightRange, attackRange;
    public bool InAttackedRange, playerInSightRange, playerInAttackRange;
    private void Awake()
    {        
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<Renderer>().material;
        mainPatrolPoint = GameObject.Find("MainPatrolArea").transform.position;
        playerSeenArea = GameObject.Find("PlayerSeenArea").transform.position; 
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public enum States
    {
        Patrol, Search, Retreat, Chase, Attack, Flee
    }
    public States states;
    // Update is called once per frame
    void Update()
    {       
        //Setting spheres detectors to check for player
        InAttackedRange = Physics.CheckSphere(transform.position, attackedRange, whatIsPlayer);
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        whatState(InAttackedRange, playerInSightRange, playerInAttackRange, states);

        // legacy code. ignore
        /*if (!playerInSightRange && !playerInAttackRange)
        {
            SearchingForPlayer();           
        }
        if (!searching)
        {
            Retreating();
        }
        if (!retreating)
        {
            Patroling();
        }
        //if (!playerInSightRange && !playerInAttackRange) whatState(States.Patrol);
        if (!playerInSightRange && fellowInSightRange)
        {           
            Alerted();
        }
        if (playerInSightRange && !playerInAttackRange) ChasingPlayer();
        if (playerInSightRange && playerInAttackRange) AttackingPlayer();*/
    }
    private void whatState(bool attacked, bool inSight, bool inAttack, States states)
    {
        if (attacked) {
            playerInSightRange = false;
            playerInAttackRange = false;
            states = States.Flee;
        } 
        if (!inSight && !inAttack) states = States.Search;
        if (inSight) states = States.Chase;
        if (inAttack) states = States.Attack;

        switch (states)
        {
            case States.Chase:
                ChasingPlayer();
                break;

            case States.Attack:
                AttackingPlayer();
                break;


            case States.Search:
                SearchingForPlayer();
                if (!searching) Retreating();
                if (!retreating) Patroling();
                break;

            case States.Flee:              
                FleeFromPlayer();
                break;
        }
    }
    private void SearchingForPlayer()
    {
        chasing = false;
        agent.SetDestination(playerSeenArea);
        if (Vector3.Distance(transform.position, playerSeenArea) < 0.5f) searching = false;
        
        material.color = Color.magenta;
        
    }
    private void FleeFromPlayer()
    {

        Vector3 dirToPlayer = transform.position - player.position;

        Vector3 newPos = transform.position + dirToPlayer; 
        agent.SetDestination(newPos);
        material.color = Color.black;   
    }
    private void Alerted()
    {
        //fellowai = GameObject.FindGameObjectsWithTag("Enemy").transform;
        enemyAI = fellowai.GetComponent<EnemyAI>();
        if (enemyAI.chasing)
        {
            agent.SetDestination(fellowai.position);
        }
    }
    private void Patroling()
    {
        Transform wp = patrolPoints[whichPatrolPoint];

        if (Vector3.Distance(transform.position, wp.position) < 1f)
        {
            whichPatrolPoint = (whichPatrolPoint + 1) % patrolPoints.Length; 
            if (whichPatrolPoint == patrolPoints.Length)
            {
                whichPatrolPoint = 0;
            }
        }
        else agent.SetDestination(wp.position);      
        
        material.color = Color.green;
    }
    private void Retreating()
    {
        agent.SetDestination(mainPatrolPoint);

        if (Vector3.Distance(transform.position, mainPatrolPoint) < 0.5f) retreating = false;
        
        material.color = Color.cyan;
    }

    private void ChasingPlayer()
    {
        playerSeenArea = player.position;
        agent.SetDestination(player.position);

        chasing = true;
        searching = true;
        retreating = true;
        
        material.color = Color.yellow;
    }
    private void AttackingPlayer()
    {
        agent.SetDestination(transform.position);

        material.color = Color.red;   
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, walkPointRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,attackedRange);
    }
}
