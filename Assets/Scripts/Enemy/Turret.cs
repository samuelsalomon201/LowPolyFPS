using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float rangeToTargetPlayer, timeBetweenShots = 0.5f;
    private float shotCounter;
    [SerializeField] private Transform gun, firePoint;
    [SerializeField] private float rotateSpeed = 45.0f;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = timeBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToTargetPlayer)
        {
            gun.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 0.7f, 0f));

            shotCounter -= Time.deltaTime;

            if (shotCounter <= 0)
            {
                Instantiate(bullet, firePoint.position, firePoint.rotation);
                shotCounter = timeBetweenShots;
            }
        }
        else
        {
            shotCounter = timeBetweenShots;

            gun.rotation = Quaternion.Lerp(gun.rotation, Quaternion.Euler(0f, gun.rotation.eulerAngles.y + 10.0f, 0f),
                rotateSpeed * Time.deltaTime);
        }
    }
}