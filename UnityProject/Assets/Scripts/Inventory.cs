using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int fireCrystalCount;
    public int iceCrystalCount;
    public int lightningCrystalCount;
    public int arcaneCrystalCount;
    public int holySealCount;
    public int evilSealCount;
    [Space]
    public Text fireCrystalText;
    public Text iceCrystalText;
    public Text lightningCrystalText;
    public Text arcaneCrystalText;
    public Text holySealText;
    public Text evilSealText;
    [Space]
    public int maxStackValue = 999;
    private void Start()
    {
        if (fireCrystalText == null)
            fireCrystalText = transform.GetChild(0).GetComponent<Text>();

        if (iceCrystalText == null)
            iceCrystalText = transform.GetChild(1).GetComponent<Text>();

        if (lightningCrystalText == null)
            lightningCrystalText = transform.GetChild(2).GetComponent<Text>();

        if (arcaneCrystalText == null)
            arcaneCrystalText = transform.GetChild(3).GetComponent<Text>();

        if (holySealText == null)
            holySealText = transform.GetChild(4).GetComponent<Text>();

        if (evilSealText == null)
            evilSealText = transform.GetChild(5).GetComponent<Text>();
    }

    private void Update()
    {
        InventoryTextUpdate();

        if (Input.GetKeyDown(KeyCode.Keypad1))
            fireCrystalCount++;
        if (Input.GetKeyDown(KeyCode.Keypad2))
            iceCrystalCount++;
        if (Input.GetKeyDown(KeyCode.Keypad3))
            lightningCrystalCount++;
        if (Input.GetKeyDown(KeyCode.Keypad4))
            arcaneCrystalCount++;
        if (Input.GetKeyDown(KeyCode.Keypad5))
            holySealCount++;
        if (Input.GetKeyDown(KeyCode.Keypad6))
            evilSealCount++;
    }

    private void InventoryTextUpdate()
    {
        if (fireCrystalText.text != fireCrystalCount.ToString())
            fireCrystalText.text = fireCrystalCount.ToString();

        if (iceCrystalText.text != iceCrystalCount.ToString())
            iceCrystalText.text = iceCrystalCount.ToString();

        if (lightningCrystalText.text != lightningCrystalCount.ToString())
            lightningCrystalText.text = lightningCrystalCount.ToString();

        if (arcaneCrystalText.text != arcaneCrystalCount.ToString())
            arcaneCrystalText.text = arcaneCrystalCount.ToString();

        if (holySealText.text != holySealCount.ToString())
            holySealText.text = holySealCount.ToString();

        if (evilSealText.text != evilSealCount.ToString())
            evilSealText.text = evilSealCount.ToString();
    }

    private void CheckMaxCount()
    {
        if (fireCrystalCount > maxStackValue)
            fireCrystalCount = maxStackValue;

        if (iceCrystalCount > maxStackValue)
            iceCrystalCount = maxStackValue;

        if (lightningCrystalCount > maxStackValue)
            lightningCrystalCount = maxStackValue;

        if (arcaneCrystalCount > maxStackValue)
            arcaneCrystalCount = maxStackValue;

        if (holySealCount > maxStackValue)
            holySealCount = maxStackValue;

        if (evilSealCount > maxStackValue)
            evilSealCount = maxStackValue;
    }

    public void LowTableLoot(int lootCount)
    {
        while (lootCount != 0)
        {
            int r = Random.Range(1, 5);
            if (r == 1)
                fireCrystalCount += 1;
            else if (r == 2)
                iceCrystalCount += 1;
            else if (r == 3)
                lightningCrystalCount += 1;
            else
                arcaneCrystalCount += 1;

            lootCount -= 1;
        }
    }
}
