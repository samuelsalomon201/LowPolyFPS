using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 30.0f;
    [SerializeField] private float lifeTime = 5.0f;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = transform.forward * moveSpeed;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealthController>().DamageEnemy(damage);
        }
        else if (other.gameObject.tag == "Target")
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
        Instantiate(impactEffect, transform.position + transform.forward * (-moveSpeed * Time.deltaTime),
            transform.rotation);
    }
}