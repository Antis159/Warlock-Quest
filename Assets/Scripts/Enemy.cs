using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public bool inCombat;
    public int maxHealth;
    public int currentHealth;
    public int damage;
    public float attackSpeed;
    public int lowLootTable;

    private Animator anim;
    private PlayerControl Player;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if(inCombat)
        {
            anim.SetBool("isAttacking", true);
            if (Player.transform.position.x > transform.position.x)
                transform.rotation = Quaternion.identity;
            else
                transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            anim.SetBool("isAttacking", false);
        }
    }

    public void StartCombat()
    {
        inCombat = true;
        StartCoroutine(SimulateCombat());
    }

    public void EndCombat()
    {
        if (currentHealth <= 0)
        {
            DestroyImmediate(gameObject);
            Player.inventory.LowTableLoot(lowLootTable);
        }

        inCombat = false;
        //Debug.Log("EndCombat");
    }
    IEnumerator SimulateCombat()
    {
        while (inCombat)
        {
            yield return new WaitForSeconds(attackSpeed);
            Player.ReceiveDamage(damage);
            if (Player.inCombat == false)
                break;
        }
        yield return null;
    }

    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void CheckHealth()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
    public string GetEnemyName()
    {
        return enemyName;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
