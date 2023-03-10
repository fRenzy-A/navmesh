using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    Material material;

    public Vector3 walkPoint;
    public Transform[] patrolPoints;
    private int whichPatrolPoint = 0;
    public Vector3 currentPatrolPoint;
    public Vector3 playerSeenArea;
    
    bool walkPointSet;
    public float walkPointRange;

    public float searchTime;

    public bool patrolsearching;
    public bool searching;
    public bool retreating;

    public float fleeRange, sightRange, attackRange;
    public bool playerInFleeRange, playerInSightRange, playerInAttackRange;
    private void Awake()
    {        
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<Renderer>().material;
        playerSeenArea = GameObject.Find("PlayerSeenArea").transform.position; 
    }

    public enum States
    {
        Patrol, Search, PatrolSearch, Retreat, Chase, Attack, Flee
    }
    public States states;
    void Update()
    {       
        //Spheres detectors to check for player
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInFleeRange = Physics.CheckSphere(transform.position, fleeRange, whatIsPlayer);

        WhichState(playerInFleeRange, playerInSightRange, playerInAttackRange, states);
    }
    private void WhichState(bool attacked, bool inSight, bool inAttack, States states)
    {
        //when player is out of the chase area
        if (!inSight && !inAttack && !attacked) states = States.Search;
        if (!searching) states = States.PatrolSearch;
        if (!patrolsearching) states = States.Retreat;
        if (!retreating) states = States.Patrol;

        if (inSight) states = States.Chase;
        if (inAttack) states = States.Attack;
        if (attacked) states = States.Flee;

        switch (states)
        {
            default:
                Patroling();
                break;
            case States.Chase:
                Chasing();
                break;
            case States.Attack:
                Attacking();
                break;
            case States.Flee:              
                Fleeing();
                break;
            case States.Search:
                Searching();
                break;
            case States.PatrolSearch:               
                SearchPatroling();
                break;
            case States.Retreat: 
                Retreating();
                break;
        }
    }
    private void Searching()
    {
        agent.SetDestination(playerSeenArea);
        if (Vector3.Distance(transform.position, playerSeenArea) < 1.0f) searching = false;

        material.color = Color.magenta;       
    }
    private void SearchPatroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        //How long patrol searching lasts for
        searchTime -= Time.deltaTime;
        if (searchTime < 0)
        {
            searchTime = 10.0f;
            patrolsearching = false;
        }

    }
    //Sets up random patrol points in a set range around enemy
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ =  Random.Range(-walkPointRange, walkPointRange);
        float randomX =  Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void Retreating()
    {
        agent.SetDestination(currentPatrolPoint);

        if (Vector3.Distance(transform.position, currentPatrolPoint) < 0.5f) retreating = false;
        
        material.color = Color.cyan;
    }
    private void Patroling()
    {
        Transform wp = patrolPoints[whichPatrolPoint];
        currentPatrolPoint = wp.position;

        if (Vector3.Distance(transform.position, wp.position) < 1f)
        {
            //When enemy goes to all patrol points
            whichPatrolPoint = (whichPatrolPoint + 1); 
            if (whichPatrolPoint == patrolPoints.Length)
            {
                whichPatrolPoint = 0;
            }
        }
        else agent.SetDestination(wp.position);      
        
        material.color = Color.green;
    }

    private void Chasing()
    {
        playerSeenArea = player.position;
        agent.SetDestination(player.position);

        searching = true;
        patrolsearching = true;
        retreating = true;

        //Resets time
        if (searching)
        {
            searchTime = 5.0f;
        }
        
        material.color = Color.yellow;
    }
    private void Attacking()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        material.color = Color.red;   
    }
    private void Fleeing()
    {
        Vector3 dirToPlayer = transform.position - player.position;

        Vector3 newPos = transform.position + dirToPlayer; 
        agent.SetDestination(newPos);
        material.color = Color.blue;   
    }
    
    //gizmos for editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,fleeRange);
    }
}
