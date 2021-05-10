using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject tileMarker;
    public Slider healthBar;
    public Slider manaBar;
    public GameObject moveButtons;
    public GameObject fleeButton;
    public GameObject skillIconParent;
    public Slider enemyHealthBar;
    public Text enemyNameText;
    public GameObject buildingButton;
    public GameObject buildingUI;
    public GameObject confirmButton;
    public GameObject cancelButton;
    public Inventory inventory;
    public GameObject buildWarning;
    public GameObject fadeScreen;
    public Text floorLevel;
    public GameObject pauseMenuButton;
    public GameObject pauseMenu;
    public GameObject clickCircle;
    public GameObject buttonCircle;
    public GameObject summoningCircleMenu;
    public GameObject textBox;
    public GameObject textCover;

    public Text healthText;
    public Text manaText;
    public Text enemyHealthText;

    private PlayerControl Player;
    public int totalBuildingCount;
    public bool isBuilding;
    private GameObject lastBuildingTemp;
    private GameObject lastBuilding;
    private GameObject lastWeapon;
    private int lastBuildingWidth;
    private int lastBuildingHeight;
    private GameObject buildingMouse;
    private Vector3 mousePos;
    public GameObject buildingSpawnParticles;
    public int lostBookRate;

    private MusicSoundController musicSoundController;

    public int fadeFrames = 20;
    public float fadeTime = 1f;
    public float sceneFadeTime = 1f;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        StartPrep();
    }

    void Update()
    {
        Health_ManaBarUpdate();

        if (SceneManager.GetActiveScene().name == "CaveScene")
            floorLevel.gameObject.SetActive(true);
        else
            floorLevel.gameObject.SetActive(false);


        if(isBuilding)
        {
            moveButtons.SetActive(false);
            buildingButton.SetActive(false);
            buildingUI.SetActive(false);
            confirmButton.SetActive(true);
            cancelButton.SetActive(true);

            if(Input.GetMouseButton(0) && !IsPointerOverUI())
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0);
                buildingMouse.transform.position = mousePos;
            }
        }
        else
        {
            if (Player.inCombat)
            {
                moveButtons.SetActive(false);
                fleeButton.SetActive(true);

                enemyHealthBar.gameObject.SetActive(true);
                enemyHealthText.gameObject.SetActive(true);
                enemyNameText.gameObject.SetActive(true);
                if (Player.GetLastEnemy() != null)
                    enemyNameText.text = Player.GetLastEnemy().enemyName;
                buildingButton.SetActive(false);
                skillIconParent.SetActive(true);
            }
            else
            {
                if(textCover.activeSelf == false)
                {
                    if (!Player.pathFindingMove)
                        moveButtons.SetActive(true);

                    if(gameObject.GetComponent<TutorialWriter>().buildingButton == true)
                    {
                        buildingButton.SetActive(true);
                        inventory.gameObject.SetActive(true);
                    }
                }

                skillIconParent.SetActive(false);
                fleeButton.SetActive(false);
                enemyHealthBar.gameObject.SetActive(false);
                enemyHealthText.gameObject.SetActive(false);
                enemyNameText.gameObject.SetActive(false);
                confirmButton.SetActive(false);
                cancelButton.SetActive(false);
            }
        }
    }

    public void Health_ManaBarUpdate()
    {
        //Player
        if (Player.GetMaxHealth() != healthBar.maxValue)
            healthBar.maxValue = Player.GetMaxHealth();

        if (healthBar.value != Player.GetCurrentHealth())
        {
            healthBar.value = Player.GetCurrentHealth();
            healthText.text = healthBar.value.ToString() + "/" + healthBar.maxValue.ToString();
        }

        if (Player.GetMaxMana() != manaBar.maxValue)
            manaBar.maxValue = Player.GetMaxMana();

        if(manaBar.value != Player.GetCurrentMana())
        {
            manaBar.value = Player.GetCurrentMana();
            manaText.text = manaBar.value.ToString() + "/" + manaBar.maxValue.ToString();
        }

        //Enemy
        if (enemyHealthBar.IsActive() == true)
        {
            if (Player.IsLastEnemy() == true)
            {
                Enemy enemy = Player.GetLastEnemy();
                if (enemyHealthBar.maxValue != enemy.GetMaxHealth())
                    enemyHealthBar.maxValue = enemy.GetMaxHealth();

                if (enemyHealthBar.value != enemy.GetCurrentHealth())
                {
                    enemyHealthBar.value = enemy.GetCurrentHealth();
                    enemyHealthText.text = enemyHealthBar.value.ToString() + "/" + enemyHealthBar.maxValue.ToString();
                }
            }
        }
    }
    public bool IsPointerOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void BuildingButton()
    {
        buildingUI.SetActive(!buildingUI.activeSelf);
    }
    public void Build()
    {
        StoreBuilding _storeBuilding = EventSystem.current.currentSelectedGameObject.GetComponent<StoreBuilding>();
        lastBuilding = _storeBuilding.GetBuilding();
        lastBuildingTemp = _storeBuilding.GetTempBuilding();
        BuildingControl buildingData = _storeBuilding.GetBuilding().GetComponent<BuildingControl>();
        int[] buildCost = buildingData.buildCost;
        lastBuildingHeight = buildingData.height;
        lastBuildingWidth = buildingData.width;
        Text[] costText = GameObject.Find("BuildingCostParent").GetComponentsInChildren<Text>();

        foreach (Text item in costText)
        {
            item.text = "0";
        }
        costText[0].text = _storeBuilding.GetBuilding().GetComponent<BuildingControl>().buildingName;
        for (int i = 1; i<costText.Length; i++)
        {
            costText[i].text = buildCost[i-1].ToString();
        }

    }

    public void BuildSelect()
    {
        int[] buildCost = lastBuilding.GetComponent<BuildingControl>().buildCost;
        if (inventory.fireCrystalCount >= buildCost[0] &&
            inventory.iceCrystalCount >= buildCost[1] &&
            inventory.lightningCrystalCount >= buildCost[2] &&
            inventory.arcaneCrystalCount >= buildCost[3] &&
            lastBuilding != null)
        {
            isBuilding = true;
            buildingMouse = Instantiate(lastBuildingTemp, Player.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
        else
        {
            StartCoroutine(BuildWarning());
        }
    }
    public void ConfirmBuild()
    {
        Player.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        if (Physics2D.OverlapCircle(new Vector2(buildingMouse.transform.position.x, buildingMouse.transform.position.y), lastBuilding.GetComponent<CircleCollider2D>().radius))
        {
            CancelBuild();
            return;
        }
        Player.gameObject.GetComponent<CircleCollider2D>().enabled = false;

        int[] buildCost = lastBuilding.GetComponent<BuildingControl>().buildCost;

        inventory.fireCrystalCount -= buildCost[0];
        inventory.iceCrystalCount -= buildCost[1];
        inventory.lightningCrystalCount -= buildCost[2];
        inventory.arcaneCrystalCount -= buildCost[3];

        isBuilding = false;
        Vector3 buildingPos = buildingMouse.transform.position;
        DestroyImmediate(buildingMouse);
        StartCoroutine(DelayBuildingSpawn(lastBuilding, buildingPos));
        Instantiate(buildingSpawnParticles, buildingPos, Quaternion.identity);
    }

    IEnumerator DelayBuildingSpawn(GameObject building, Vector3 buildingPos)
    {
        yield return new WaitForSeconds(.5f);
        GameObject spawnedBuilding = Instantiate(building, buildingPos, Quaternion.identity);
        spawnedBuilding.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
        Player.GetComponent<SaveLoadGame>().SaveBaseBuildings();
    }

    public void CancelBuild()
    {
        isBuilding = false;
        DestroyImmediate(buildingMouse);
    }

    public void FadeScreen(float? fadeTime = null)
    {
        StartCoroutine(FadeCoroutine(fadeTime));
    }
    IEnumerator FadeCoroutine(float? fadeTime = null)
    {
        fadeScreen.SetActive(true);
        if (fadeTime == null)
            fadeTime = 0.5f;
        Image fadeImg = fadeScreen.GetComponent<Image>();

        for(int i=0; i<20; i++)
        {
            fadeImg.color = new Color(0,0,0,0.05f * (i+1));
            yield return new WaitForSeconds(0.015f);
        }

        yield return new WaitForSeconds((float)fadeTime);

        for(int i=0; i<20; i++)
        {
            fadeImg.color = new Color(0, 0, 0, 1f - (i+1) * 0.05f);
            yield return new WaitForSeconds(0.015f);
        }
        fadeScreen.SetActive(false);
    }

    IEnumerator BuildWarning()
    {
        buildWarning.gameObject.SetActive(true);
        Text text = buildWarning.GetComponentInChildren<Text>();
        Image image = buildWarning.GetComponent<Image>();

        float startTextAlpha = text.color.a;
        float startImageAlpha = image.color.a;

        float textFadePerFrame = startTextAlpha / fadeFrames;
        float imageFadePerFrame = startImageAlpha / fadeFrames;

        while(true)
        {
            for(int i=0;i<fadeFrames * 0.5f;i++)
            {
                text.color = new Color(1,1,1,startTextAlpha - textFadePerFrame * i);
                image.color = new Color(0, 0, 0, startImageAlpha - imageFadePerFrame * i);

                yield return new WaitForSeconds(fadeTime / (float)fadeFrames);
            }

            buildWarning.gameObject.SetActive(false);
            text.color = new Color(1, 1, 1, startTextAlpha);
            image.color = new Color(0, 0, 0, startImageAlpha);
            break;
        }
    }

    public void PauseMenuButton()
    {
        pauseMenu.SetActive(true);
    }

    public void MusicToggle()
    {
        musicSoundController.musicToggle = !musicSoundController.musicToggle;
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = musicSoundController.GetMusicCheckboxSprite();
    }

    public void SoundToggle()
    {
        musicSoundController.soundToggle = !musicSoundController.soundToggle;
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = musicSoundController.GetSoundCheckboxSprite();
    }

    public void PauseMenuExit()
    {
        pauseMenu.SetActive(false);
    }

    public void ClearProgressButton()
    {
        DDOL_manager.Restart();
    }

    public void MovementToggle()
    {
        Player.pathFindingMove = !Player.pathFindingMove;
        MovementToggleCircleCheck();
    }

    public void MovementToggleCircleCheck()
    {
        if (Player.pathFindingMove == true)
        {
            clickCircle.SetActive(true);
            buttonCircle.SetActive(false);
            moveButtons.SetActive(false);
        }
        else
        {
            clickCircle.SetActive(false);
            buttonCircle.SetActive(true);
        }
    }

    public void OrbSelect()
    {
        StoreOrb _storeOrb = EventSystem.current.currentSelectedGameObject.GetComponent<StoreOrb>();
        lastWeapon = _storeOrb.GetOrb();
        Weapon weaponData = _storeOrb.GetOrb().GetComponent<Weapon>();
        int[] summonCost = weaponData.summonCost;
        Text[] costText = GameObject.Find("OrbCostParent").GetComponentsInChildren<Text>();

        foreach (Text item in costText)
        {
            item.text = "0";
        }
        costText[0].text = _storeOrb.GetOrb().GetComponent<Weapon>().weaponName;
        for (int i = 1; i < costText.Length; i++)
        {
            costText[i].text = summonCost[i - 1].ToString();
        }

    }
    public void SummoningCircleMenuActive()
    {
        summoningCircleMenu.gameObject.SetActive(!summoningCircleMenu.activeSelf);
    }

    public void LostBookRoll()
    {
        if (Player.buildingsUnlocked < totalBuildingCount && Random.Range(0, lostBookRate) == 0)
        {
            Player.buildingsUnlocked++;
            Player.GetComponent<SaveLoadGame>().SaveBuildingUnlocks();
            BuildingSlotCheck();

        }
        else
        {
            inventory.GetComponent<Inventory>().LowTableLoot(5);
        }
    }

    public void BuildingSlotCheck()
    {
        for (int i=0; i< Player.buildingsUnlocked; i++)
        {
             buildingUI.transform.GetChild(0).GetChild(i + 1).gameObject.SetActive(true);
        }
    }

    public void SummonWeapon()
    {
        Player.EquipWeapon(lastWeapon);
        SummoningCircleMenuActive();
    }

    public void TutorialStart()
    {
        healthBar.gameObject.SetActive(false);
        manaBar.gameObject.SetActive(false);
        inventory.gameObject.SetActive(false);
        buildingButton.SetActive(false);
        moveButtons.SetActive(false);
        textCover.SetActive(true);
    }
    public void TurnTextOff()
    {
        textBox.SetActive(false);
        textCover.SetActive(false);
        healthBar.gameObject.SetActive(true);
        manaBar.gameObject.SetActive(true);
        if(gameObject.GetComponent<TutorialWriter>().buildingButton == true)
        {
            inventory.gameObject.SetActive(true);
            buildingButton.SetActive(true);
            moveButtons.SetActive(true);
        }
        gameObject.GetComponent<TutorialWriter>().textCount = 0;
    }

    public void StartPrep()
    {

        if (healthBar == null)
            healthBar = transform.GetChild(0).gameObject.GetComponent<Slider>();

        if (manaBar == null)
            manaBar = transform.GetChild(1).gameObject.GetComponent<Slider>();

        if (moveButtons == null)
            moveButtons = transform.GetChild(2).gameObject;

        if (fleeButton == null)
            fleeButton = transform.GetChild(3).gameObject;

        if (enemyHealthBar == null)
            enemyHealthBar = transform.GetChild(4).gameObject.GetComponent<Slider>();

        if (enemyNameText == null)
            enemyNameText = transform.GetChild(5).gameObject.GetComponent<Text>();

        if (healthText == null)
            healthText = healthBar.GetComponentInChildren<Text>();

        if (manaText == null)
            manaText = manaBar.GetComponentInChildren<Text>();

        if (enemyHealthText == null)
            enemyHealthText = enemyHealthBar.GetComponentInChildren<Text>();

        if (buildingButton == null)
            buildingButton = transform.GetChild(6).gameObject;

        if (buildingUI == null)
            buildingUI = transform.GetChild(7).gameObject;

        if (confirmButton == null)
            confirmButton = transform.GetChild(8).gameObject;

        if (cancelButton == null)
            cancelButton = transform.GetChild(9).gameObject;

        if (inventory == null)
            inventory = transform.GetChild(10).GetComponent<Inventory>();

        if (buildWarning == null)
            buildWarning = transform.GetChild(11).gameObject;

        if (floorLevel == null)
            floorLevel = transform.GetChild(12).gameObject.GetComponent<Text>();

        if (pauseMenuButton == null)
            pauseMenuButton = transform.GetChild(13).gameObject;

        if (pauseMenu == null)
            pauseMenu = transform.GetChild(14).gameObject;

        if (summoningCircleMenu == null)
            summoningCircleMenu = transform.GetChild(15).gameObject;

        if (textBox == null)
            textBox = transform.GetChild(16).gameObject;

        if (textCover == null)
            textCover = transform.GetChild(17).gameObject;

        if (skillIconParent == null)
            skillIconParent = transform.GetChild(18).gameObject;

        if (fadeScreen == null)
            fadeScreen = transform.GetChild(-1).gameObject;

        musicSoundController = GetComponent<MusicSoundController>();
        enemyHealthBar.gameObject.SetActive(false);
        enemyHealthText.gameObject.SetActive(false);
        enemyNameText.gameObject.SetActive(false);
        buildingUI.SetActive(false);
        confirmButton.SetActive(false);
        cancelButton.SetActive(false);
        inventory.gameObject.SetActive(true);
        buildWarning.SetActive(false);
        floorLevel.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
        summoningCircleMenu.SetActive(false);
        textBox.SetActive(false);
        textCover.SetActive(false);
        skillIconParent.SetActive(false);

        BuildingSlotCheck();
        MovementToggleCircleCheck();
    }
}
