using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    Material material;

    public Vector3 walkPoint;
    public Vector3 searchWalkPoint;

    bool walkPointSet, searchWalkPointSet;
    public float walkPointRange, searchWalkPointRange;

    public float chasingTime = 1100.0f;

    bool chasing;
    bool searching;

    public float searchRange, sightRange, attackRange;
    public bool playerInSearchRange, playerInSightRange, playerInAttackRange;
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<Renderer>().material;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }   

    // Update is called once per frame
    void Update()
    {

        playerInSearchRange = Physics.CheckSphere(transform.position, searchRange, whatIsPlayer);
        playerInSightRange = Physics.CheckSphere(transform.position, searchRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && !playerInSearchRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (!playerInSightRange && playerInSearchRange) SearchPlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }


    private void Patroling()
    {
        if (!walkPointSet) MainSearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        material.color = Color.green;
    }

    private void SearchPlayer()
    {
        searching = true;
        if (!searchWalkPointSet) SearchWalkPoint();

        if (searchWalkPointSet)
            agent.SetDestination(searchWalkPoint);

        Vector3 distanceToWalkPoint = player.position - searchWalkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            searchWalkPointSet = false;

        material.color= Color.magenta;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);


        material.color = Color.yellow;
    }


    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        material.color = Color.red;   
    }
    private void MainSearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-searchWalkPointRange, searchWalkPointRange);
        float randomX = Random.Range(-searchWalkPointRange, searchWalkPointRange);

        searchWalkPoint = new Vector3(player.position.x + randomX, player.position.y, player.position.z + randomZ);

        if (Physics.Raycast(searchWalkPoint, -transform.up, 2f, whatIsGround))
            searchWalkPointSet = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
}
