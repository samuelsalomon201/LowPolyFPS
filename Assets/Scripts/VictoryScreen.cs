using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private string MainMenuScene = "MainMenu";

    [SerializeField] private float timeBetweenShowing = 1.0f;

    [SerializeField] private GameObject textBox, returnButton;

    [SerializeField] private Image blackScreen;

    [SerializeField] private float blackScreenFade = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowObjectsCo());
    }

    // Update is called once per frame
    void Update()
    {
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.b,
            Mathf.MoveTowards(blackScreen.color.a, 0f, blackScreenFade * Time.deltaTime));
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

    public IEnumerator ShowObjectsCo()
    {
        yield return new WaitForSeconds(timeBetweenShowing);

        textBox.SetActive(true);

        yield return new WaitForSeconds(timeBetweenShowing);

        returnButton.SetActive(true);
    }
}