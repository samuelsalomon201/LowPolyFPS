using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int damage = 25;
    [SerializeField] private bool damageEnemy, damagePlayer;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && damageEnemy)
        {
            other.gameObject.GetComponent<EnemyHealthController>().DamageEnemy(damage);
        }

        else if (other.gameObject.tag == "Player" && damagePlayer)
        {
            Debug.Log("Hit Player At " + transform.position);
            PlayerHealthController.instance.DamagePlayer(damage);
        }
    }
}
