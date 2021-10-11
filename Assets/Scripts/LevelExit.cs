using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    [SerializeField] private float waitToEndLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.ending = true;
            StartCoroutine(EndLevelCo());
            
            AudioManager.instance.PlayLevelVictory();
        }
    }

    private IEnumerator EndLevelCo()
    {
        PlayerPrefs.SetString(nextLevel + "_cp", "");
        
        yield return new WaitForSeconds(waitToEndLevel);
        
        SceneManager.LoadScene(nextLevel);
    }
}
