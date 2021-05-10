using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    public bool skipTutorial = false;
    private void Awake()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        while(!SceneManager.GetActiveScene().isLoaded)
        {
            yield return new WaitForSeconds(0.01f);
        }

        if(!skipTutorial)
            SceneManager.LoadScene("TutorialScene");
        else
            SceneManager.LoadScene("HomeScene");

        yield return new WaitForSeconds(0.1f);
    }
}
