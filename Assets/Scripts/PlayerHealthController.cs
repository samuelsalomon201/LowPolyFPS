using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    [SerializeField] private int maxHealth, currentHealth;
    [SerializeField] private float invincibleLenght = 1.0f;
    private float invincCounter;

    private void Awake()
    {
        instance = this;
    }

        // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (invincCounter > 0)
        {
            invincCounter -= Time.deltaTime;
        }
    }

    public void DamagePlayer(int damageAmount)
    {
        if (invincCounter <= 0 &&  !GameManager.instance.ending)
        {
            AudioManager.instance.PlaySFX(6);
            currentHealth -= damageAmount;

            UIController.instance.ShowDamage();
            
            if (currentHealth <= 0)
            {
                gameObject.SetActive(false);
                Debug.Log("Player Health Is 0");

                currentHealth = 0;
                
                GameManager.instance.PlayerDied();
                
                AudioManager.instance.StopBGM();
                AudioManager.instance.PlaySFX(5);
                AudioManager.instance.StopSFX(6);
            }

            invincCounter = invincibleLenght;
            
            UIController.instance.healthSlider.value = currentHealth;
            UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "Health: " + currentHealth + "/" + maxHealth;
    }
}
