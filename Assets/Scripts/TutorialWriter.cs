using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialWriter : MonoBehaviour
{
    private GameObject player;
    private Inventory inventory;
    private Vector3 enemyTextTriggerPosition = new Vector3(14, -10, 0);
    private Vector3 portalTextTriggerPosition = new Vector3(22, -10, 0);
    private Text messageText;
    private UIController _UIController;
    private TextWriter.TextWriterSingle textWriterSingle;
    public int textCount;
    private bool textLeft = true;
    private bool firstEnemyTrigger = false;
    private bool portalTrigger = false;
    public bool buildingButton = false;
    string[] textArray = new string[] 
    {
        "Ughh... My head...",
        "These surroundings don't look familiar. Where am I?",
        "I guess talking to my self won't help. Time to try and find a way out of this cave",
    };

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<PlayerControl>().inventory;
        messageText = transform.Find("TextBox").Find("MainText").GetComponent<Text>();
        _UIController = gameObject.GetComponent<UIController>();
        if(!PlayerPrefs.HasKey("tutorialComplete"))
        {
            inventory.arcaneCrystalCount = 35;
            inventory.lightningCrystalCount = 35;
            inventory.fireCrystalCount = 70;
            inventory.iceCrystalCount = 70;
            _UIController.TutorialStart();
            WriteText();
        }
    }
    private void Update()
    {
        if (Vector3.Distance(player.transform.position, enemyTextTriggerPosition) < 1f && firstEnemyTrigger == false)
            FirstEnemyTextTrigger();

        if (Vector3.Distance(player.transform.position, portalTextTriggerPosition) < 1f && portalTrigger == false)
            PortalHomeTrigger();
    }

    public void FirstEnemyTextTrigger()
    {
        buildingButton = true;
        player.GetComponent<PlayerControl>().moveDir = Vector3.zero;
        player.GetComponent<PlayerControl>().ResetAllButtons();
        firstEnemyTrigger = true;
        textArray = new string[]
        {
            "Unfamiliar and most likely aggresive creature up ahead.",
            "I should summon my self a weapon to fight with first.",
            "Although my memory is fuzzy I think I remember how to make a summoning circle",
        };

        textLeft = true;
        _UIController.TutorialStart();
        WriteText();
    }

    public void PortalHomeTrigger()
    {
        player.GetComponent<PlayerControl>().moveDir = Vector3.zero;
        player.GetComponent<PlayerControl>().ResetAllButtons();
        portalTrigger = true;
        textArray = new string[]
        {
            "That portal should bring me back home",
        };

        textLeft = true;
        _UIController.TutorialStart();
        WriteText();
    }

    public void NoWeaponText()
    {
        player.GetComponent<PlayerControl>().ResetAllButtons();
        textArray = new string[]
        {
           "I should first get my self a weapon to fight with",
        };
        textLeft = true;
        _UIController.TutorialStart();
        WriteText();
    }

    public void WriteText()
    {
        if (_UIController.textBox.activeSelf == false)
            _UIController.textBox.SetActive(true);

        if (textWriterSingle != null && textWriterSingle.IsActive())
        {
            textWriterSingle.WriteAllAndDestroy();
        }
        else
        {
            if (textCount >= textArray.Length)
            {
                textLeft = false;
                _UIController.TurnTextOff();
            }
            if (textLeft)
            {
                string message = textArray[textCount];
                textWriterSingle = TextWriter.AddWriter_Static(messageText, message, 0.05f, true, true);
                textCount++;
            }
        }
    }
}
