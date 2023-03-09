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
    public float walkPointRange;

    public float chasingTime = 1100.0f;

    public bool searching;
    public bool retreating;
    public bool chasing;

    public float searchTime = 10.0f;
    public float searchTimeLeft;

    public float alertRange, sightRange, attackRange;
    public bool fellowInSightRange, playerInSightRange, playerInAttackRange;
    private void Awake()
    {
        
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<Renderer>().material;
        mainPatrolPoint = GameObject.Find("MainPatrolArea").transform.position;
        playerSeenArea = GameObject.Find("PlayerSeenArea").transform.position; 
        searchTimeLeft = searchTime;
    }
    // Start is called before the first frame update
    void Start()
    {
        searching = false;
    }
    public enum States
    {
        Patrol, Search, Retreat, Chase, Attack
    }
    public States states;
    // Update is called once per frame
    void Update()
    {       
        fellowInSightRange = Physics.CheckSphere(transform.position, alertRange);
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
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
        if (playerInSightRange && playerInAttackRange) AttackingPlayer();
    }
    private void whatState(States states)
    {
        switch (states)
        {
            case States.Search:
                SearchingForPlayer();
                if (!searching)
                {
                    states = States.Retreat;
                }
                break;
            case States.Retreat:
                Retreating();
                if (!retreating)
                {
                    states = States.Patrol;
                }
                break;
            case States.Patrol:
                Patroling();
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
    private void Alerted()
    {
        fellowai = GameObject.FindGameObjectsWithTag("Enemy").transform;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,alertRange);
    }
}
