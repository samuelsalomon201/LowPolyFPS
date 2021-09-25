using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private string TheGun;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !collected)
        {
            PlayerController.instance.AddGun(TheGun);

            Destroy(gameObject);

            collected = true;
            AudioManager.instance.PlaySFX(3);
        }
    }
}