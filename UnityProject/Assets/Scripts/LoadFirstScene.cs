using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
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

        SceneManager.LoadScene("HomeScene");

        while(!SceneManager.GetActiveScene().isLoaded)
        {
            yield return new WaitForSeconds(.01f);
        }

        yield return new WaitForSeconds(0.1f);
    }
}
