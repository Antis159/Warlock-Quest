using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingControl : MonoBehaviour
{
    public string buildingName;
    public int[] buildCost = new int[4];
    public int width;
    public int height;

    [Space]
    public GameObject lBookDestroyParticles;
    public GameObject poolDestroyParticles;

    public void BuildingAction(UIController ui)
    {
        if(buildingName == "Summoning Circle")
        {
            ui.SummoningCircleMenuActive();
        }
        if(buildingName == "Pool of Blood")
        {
            PlayerControl player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
            player.currentHealth = player.maxHealth;
            Instantiate(poolDestroyParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        if(buildingName == "Floaty Book")
        {
            Debug.Log("Floaty Book");
        }
        if(buildingName == "Lost Book")
        {
            ui.LostBookRoll();
            Instantiate(lBookDestroyParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    public int[] GetBuildCost()
    {
        return buildCost;
    }
    public string GetBuildingName()
    {
        return buildingName;
    }
}
