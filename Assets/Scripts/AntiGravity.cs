using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravity : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.LowerGravity();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.GravityBack();
            }
        }
    }
}
