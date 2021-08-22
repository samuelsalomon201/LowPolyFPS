using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private bool chasing;
    private float distanceToChase = 10.0f, distanceToLose = 15.0f, distanceToStop = 2.0f;
    private float chaseCounter;
    private Vector3 targetPoint, startPoint;
    [SerializeField] private float keepChasingTime = 5.0f;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate, waitBetweenShots, timeToShoot = 1.0f;
    private float fireCount, shotWaitCounter, shootTimeCounter;

    void Start()
    {
        startPoint = transform.position;

        shootTimeCounter = timeToShoot;
        shotWaitCounter = waitBetweenShots;
    }

    void Update()
    {
        ChaseAndShoot();
    }

    void ChaseAndShoot()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;

        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;
                shootTimeCounter = timeToShoot;
                shotWaitCounter = waitBetweenShots;
            }

            if (chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter <= 0)
                {
                    agent.destination = startPoint;
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
            {
                agent.destination = targetPoint;
            }
            else
            {
                agent.destination = transform.position;
            }

            if (Vector3.Distance(transform.position, targetPoint) > distanceToLose)
            {
                chasing = false;

                chaseCounter = keepChasingTime;
            }

            if (shotWaitCounter > 0)
            {
                shotWaitCounter -= Time.deltaTime;

                if (shotWaitCounter <= 0)
                {
                    shootTimeCounter = timeToShoot;
                }
            }
            else
            {
                shootTimeCounter -= Time.deltaTime;

                if (shootTimeCounter > 0)
                {
                    fireCount -= Time.deltaTime;

                    if (fireCount <= 0)
                    {
                        fireCount = fireRate;

                        firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 1.5f, 0f));
                        
                        // check the angle to player

                        Instantiate(bullet, firePoint.position, firePoint.rotation);
                    }

                    agent.destination = transform.position;
                }
                else
                {
                    shotWaitCounter = waitBetweenShots;
                }
            }
        }
    }
}