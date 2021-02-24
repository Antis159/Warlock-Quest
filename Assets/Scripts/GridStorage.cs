using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridStorage : MonoBehaviour
{
    public GameObject[] houseGridChildren = new GameObject[100];
    public GameObject[] homeGridChildren;


    public void StoreGrid()
    {

        GameObject grid = GameObject.FindGameObjectWithTag("Grid");

        //if (grid.transform.childCount > 0)
        //{

        //    if (SceneManager.GetActiveScene().name == "HouseScene")
        //    {
        //        for (int i = 0; i < grid.transform.childCount; i++)
        //        {
        //            Debug.Log(grid.transform.GetChild(i).gameObject.name);
        //            houseGridChildren[i] = grid.transform.GetChild(i).gameObject;
        //        }
        //        Debug.Log("HouseScene Stored");
        //        Debug.Log("HouseScene childCount = " + houseGridChildren.Length);
        //    }
        //    else if (SceneManager.GetActiveScene().name == "HomeScene")
        //    {
        //        for (int i = 0; i < grid.transform.childCount; i++)
        //        {
        //            homeGridChildren[i] = grid.transform.GetChild(i).gameObject;
        //        }
        //        Debug.Log("HomeScene Stored");
        //        Debug.Log("HomeScene childCount = " + homeGridChildren.Length);
        //    }
        //}
    }

    public void LoadGrid()
    {
        GameObject grid = GameObject.FindGameObjectWithTag("Grid");

        if (SceneManager.GetActiveScene().name == "HouseScene")
        {
            for(int i=0; i<houseGridChildren.Length;i++)
            {
                GameObject child = Instantiate(houseGridChildren[i]);
                child.transform.parent = grid.transform;
            }

            Debug.Log("HouseScene Loaded");
        }
        else if (SceneManager.GetActiveScene().name == "HomeScene")
        {
            for (int i = 0; i < homeGridChildren.Length; i++)
            {
                GameObject child = Instantiate(homeGridChildren[i]);
                child.transform.parent = grid.transform;
            }

            Debug.Log("HomeScene Loaded");
        }
    }
}
