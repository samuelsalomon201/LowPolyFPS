using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8.0f;
    [SerializeField] private Rigidbody rigidBody;
    private bool chasing;
    private float distanceToChase = 10.0f, distanceToLose = 15.0f;

    private Vector3 targetPoint, startPoint;

    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private float keepChasingTime;
    private float chaseCounter;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;
        
        
        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;
            }
        }
        else
        {
            //transform.LookAt(targetPoint);

            //rigidBody.velocity = transform.forward * moveSpeed;

            agent.destination = targetPoint;

            if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)
            {
                chasing = false;

                agent.destination = startPoint;
            }
        }
    }
}
