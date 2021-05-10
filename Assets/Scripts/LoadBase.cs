using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBase : MonoBehaviour
{
    public GameObject[] allBuildings;
    public GameObject parent;
    private int buildingIndex;
    void Start()
    {
        parent = GameObject.FindGameObjectWithTag("Grid");

        if(SceneManager.GetActiveScene().name == "HomeScene")
            LoadHomeBuildings();

        if(SceneManager.GetActiveScene().name == "HouseScene")
            LoadHouseBuildings();
    }

    public void LoadHomeBuildings()
    {
        if(PlayerPrefs.HasKey("homeGridChildren"))
        {
            int n = PlayerPrefs.GetInt("homeGridChildren");

            for(int i=0; i<n; i++)
            {
                LoadBuilding(PlayerPrefs.GetString($"homeChild{i}Name"),
                             PlayerPrefs.GetFloat($"homeChild{i}PositionX"),
                             PlayerPrefs.GetFloat($"homeChild{i}PositionY"));
            }
        }
    }

    public void LoadHouseBuildings()
    {
        if (PlayerPrefs.HasKey("houseGridChildren"))
        {
            int n = PlayerPrefs.GetInt("houseGridChildren");

            for (int i = 0; i < n; i++)
            {
                LoadBuilding(PlayerPrefs.GetString($"houseChild{i}Name"),
                             PlayerPrefs.GetFloat($"houseChild{i}PositionX"),
                             PlayerPrefs.GetFloat($"houseChild{i}PositionY"));
            }
        }
    }

    public void LoadBuilding(string name, float x, float y)
    {
        if(name.Contains("SummoningCircle"))
            buildingIndex = 0;

        if(name.Contains("FloatyBook"))
            buildingIndex = 1;

        if(name.Contains("PoolOfBlood"))
            buildingIndex = 2;

        GameObject spawnGo = Instantiate(allBuildings[buildingIndex], new Vector3(x, y, 0), Quaternion.identity);
        spawnGo.transform.SetParent(parent.transform);
    }
}
