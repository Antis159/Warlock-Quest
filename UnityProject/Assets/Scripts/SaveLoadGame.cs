using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadGame : MonoBehaviour
{
    public PlayerControl Player;
    public Inventory inventory;
    public GameObject playerMoveGo;
    public bool generateCave;
    private void Awake()
    {
        if (Player == null)
            Player = GetComponent<PlayerControl>();

        if (inventory == null)
            inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        if (playerMoveGo == null)
            playerMoveGo = GameObject.Find("PlayerMove");

        generateCave = true;
        if (PlayerPrefs.HasKey("scene"))
            if (PlayerPrefs.GetString("scene") == "CaveScene")
                generateCave = false;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("scene"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("scene"));
            Player.transform.position = new Vector3(PlayerPrefs.GetFloat("playerPositionX"), PlayerPrefs.GetFloat("playerPositionY"), 0);
            playerMoveGo.transform.position = new Vector3(PlayerPrefs.GetFloat("playerPositionX"), PlayerPrefs.GetFloat("playerPositionY"), 0);

            inventory.fireCrystalCount = PlayerPrefs.GetInt("fireCrystalCount");
            inventory.iceCrystalCount = PlayerPrefs.GetInt("iceCrystalCount");
            inventory.lightningCrystalCount = PlayerPrefs.GetInt("lightningCrystalCount");
            inventory.arcaneCrystalCount = PlayerPrefs.GetInt("arcaneCrystalCount");
            inventory.holySealCount = PlayerPrefs.GetInt("holySealCount");
            inventory.evilSealCount = PlayerPrefs.GetInt("evilSealCount");

            Player.currentHealth = PlayerPrefs.GetInt("playerCurrentHealth");
            Player.maxHealth = PlayerPrefs.GetInt("playerMaxHealth");
            Player.currentMana = PlayerPrefs.GetInt("playerCurrentMana");
            Player.maxMana = PlayerPrefs.GetInt("playerMaxMana");

            if (PlayerPrefs.GetString("scene") == "CaveScene")
            {
                StartCoroutine(CaveSceneDelayedLoad());
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("playerPositionX", Player.transform.position.x);
        PlayerPrefs.SetFloat("playerPositionY", Player.transform.position.y);
        PlayerPrefs.SetInt("playerCurrentHealth", Player.GetCurrentHealth());
        PlayerPrefs.SetInt("playerMaxHealth", Player.GetMaxHealth());
        PlayerPrefs.SetInt("playerCurrentMana", Player.currentMana);
        PlayerPrefs.SetInt("playerMaxMana", Player.maxMana);
        PlayerPrefs.SetString("scene", SceneManager.GetActiveScene().name);

        PlayerPrefs.SetInt("fireCrystalCount", inventory.fireCrystalCount);
        PlayerPrefs.SetInt("iceCrystalCount", inventory.iceCrystalCount);
        PlayerPrefs.SetInt("lightningCrystalCount", inventory.lightningCrystalCount);
        PlayerPrefs.SetInt("arcaneCrystalCount", inventory.arcaneCrystalCount);
        PlayerPrefs.SetInt("hoylSealCount", inventory.holySealCount);
        PlayerPrefs.SetInt("evilSealCount", inventory.evilSealCount);

        PlayerPrefs.SetInt("currentFloorLevel", Player.currentFloorLevel);

        if (SceneManager.GetActiveScene().name == "CaveScene")
        {
            GameObject grid = GameObject.FindGameObjectWithTag("Grid");
            PlayerPrefs.SetInt("gridChildren", grid.transform.childCount);

            for(int i = 0; i<grid.transform.childCount; i++)
            {
                PlayerPrefs.SetString($"child{i}Name", grid.transform.GetChild(i).name);
                PlayerPrefs.SetFloat($"child{i}PositionX", grid.transform.GetChild(i).position.x);
                PlayerPrefs.SetFloat($"child{i}PositionY", grid.transform.GetChild(i).position.y);
            }
        }

        PlayerPrefs.Save();
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
    }

    IEnumerator CaveSceneDelayedLoad()
    {
        while(true)
        {
            if (SceneManager.GetActiveScene().name == "CaveScene")
            {
                CaveControl caveControl = GameObject.Find("SceneStartGO").GetComponent<CaveControl>();

                for (int i = 0; i < PlayerPrefs.GetInt("gridChildren"); i++)
                {
                    caveControl.LoadGO(PlayerPrefs.GetString($"child{i}Name"),
                                       PlayerPrefs.GetFloat($"child{i}PositionX"),
                                       PlayerPrefs.GetFloat($"child{i}PositionY"));
                }

                break;
            }
            yield return new WaitForSeconds(.01f);
        }
        Player.currentFloorLevel = PlayerPrefs.GetInt("currentFloorLevel");
        generateCave = true;
        yield return null;
    }
}
