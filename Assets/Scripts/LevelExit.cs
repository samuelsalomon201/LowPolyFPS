using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    [SerializeField] private float waitToEndLevel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.ending = true;
            StartCoroutine(EndLevelCo());
        }
    }

    private IEnumerator EndLevelCo()
    {
        yield return new WaitForSeconds(waitToEndLevel);
        
        SceneManager.LoadScene(nextLevel);
    }
}
