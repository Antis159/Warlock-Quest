using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CaveControl : MonoBehaviour
{
    public GameObject[] caveLayout = new GameObject[10];
    public GameObject caveExit;
    public GameObject[] availableEnemies;
    public int enemyCount;
    public GameObject parent;
    private Vector3 offset = new Vector3(.5f, .5f, 0);

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<SaveLoadGame>().generateCave == true)
        {
            GameObject spawnGo = Instantiate(caveLayout[Random.Range(0, 9)], offset, Quaternion.identity);
            spawnGo.transform.SetParent(parent.transform);
            while (true)
            {
                Vector3 spawnSpot = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
                if (!Physics2D.OverlapCircle(spawnSpot, 0.2f) && spawnSpot != Vector3.zero)
                {
                    GameObject spawnGO = Instantiate(caveExit, spawnSpot, Quaternion.identity);
                    spawnGO.transform.SetParent(parent.transform);
                    break;
                }
            }

            if (enemyCount == 0)
                enemyCount = Random.Range(5, 15);

            while (enemyCount > 0)
            {
                Vector3 spawnSpot = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
                if (!Physics2D.OverlapCircle(spawnSpot, 0.2f) && spawnSpot != Vector3.zero)
                {
                    GameObject spawnGO = Instantiate(availableEnemies[Random.Range(0, availableEnemies.Length)], spawnSpot, Quaternion.identity);
                    spawnGO.transform.SetParent(parent.transform);
                    enemyCount -= 1;
                }
            }
        }
    }

    public void LoadGO(string name, float x, float y)
    {
        if(name.Contains("Cave"))
        {
            Match match = Regex.Match(name, @"\d");
            GameObject spawnGO = Instantiate(caveLayout[int.Parse(match.Value) - 1], offset, Quaternion.identity);
            spawnGO.transform.SetParent(parent.transform);
        }

        if(name.Contains("Entrance"))
        {
            GameObject spawnGo = Instantiate(caveExit, new Vector3(x, y, 0), Quaternion.identity);
            spawnGo.transform.SetParent(parent.transform);
        }

        if(name.Contains("Enemy"))
        {
            Match match = Regex.Match(name, @"\d");
            GameObject spawnGO = Instantiate(availableEnemies[int.Parse(match.Value) - 1], new Vector3(x, y, 0), Quaternion.identity);
            spawnGO.transform.SetParent(parent.transform);
        }
    }
}
