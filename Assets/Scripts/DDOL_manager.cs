using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DDOL_manager
{
    public static List<GameObject> ddolGameObjects = new List<GameObject>();

    public static void AddToDDOL(GameObject go)
    {
        Object.DontDestroyOnLoad(go);
        ddolGameObjects.Add(go);
    }

    public static void DestroyAll()
    {
        foreach (var item in ddolGameObjects)
        {
            if (item != null)
                Object.Destroy(item);
        }

        ddolGameObjects.Clear();
    }

    public static void Restart()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<SaveLoadGame>().ClearSave();
        DestroyAll();
        SceneManager.LoadScene("MainMenuScene");
    }
}
