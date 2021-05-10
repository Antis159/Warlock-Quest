using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSimulate : MonoBehaviour
{
    private Enemy enemy;
    private GameObject enemyAttackParticles;
    private float enemyAttackSpeed;
    private float enemyAttackDamage;


    private PlayerControl player;
    private GameObject playerAttackParticles;
    private float playerAttackSpeed;
    private float playerAttackDamage;
    private bool fleePressed = false;
    private float fleeTime = 1f;

    private float playerTimer;
    private float enemyTimer;

    private void Start()
    {
        enemy.inCombat = true;
        player.inCombat = true;
    }
    private void Update()
    {
        CheckHealth();
        if(fleePressed && playerTimer >= fleeTime)
        {
            CombatEnd();
        }
        else if(playerTimer >= playerAttackSpeed && !fleePressed)
        {
            playerTimer -= playerAttackSpeed;
            GameObject tempGO = Instantiate(playerAttackParticles, player.transform.GetChild(0).transform.position, Quaternion.identity);
            AttackParticleMove attackGO = tempGO.GetComponent<AttackParticleMove>();
            attackGO.SetMove(enemy.transform.position);
            attackGO.SetEnemy(enemy);
            attackGO.SetDamage((int)playerAttackDamage); //Deal damage
        }
        else
        {
            playerTimer += Time.deltaTime;
        }

        if(enemyTimer >= enemyAttackSpeed)
        {
            enemyTimer -= enemyAttackSpeed;
            if (enemyAttackParticles != null)
                Instantiate(enemyAttackParticles, player.transform.position, Quaternion.identity);
            player.ReceiveDamage((int)enemyAttackDamage);
        }
        else
        {
            enemyTimer += Time.deltaTime;
        }

    }

    public void CheckHealth()
    {
        if(player.currentHealth <= 0 || enemy.currentHealth <= 0)
        {
            CombatEnd();
        }
    }
    public void CombatEnd()
    {
        if (player.currentHealth <= 0)
            player.Death();
        if (enemy.currentHealth <= 0)
            enemy.Death();
        player.inCombat = false;
        enemy.inCombat = false;
        DestroyImmediate(gameObject);
    }

    public void setEnemy(Enemy currentEnemy)
    {
        enemy = currentEnemy;
        if (currentEnemy.attackParticles != null)
            enemyAttackParticles = currentEnemy.attackParticles;
        enemyAttackDamage = enemy.damage;
        enemyAttackSpeed = enemy.attackSpeed;
    }

    public void setPlayer(PlayerControl currentPlayer)
    {
        player = currentPlayer;
        playerAttackParticles = currentPlayer.weaponAttackParticles;
        playerAttackDamage = player.weaponDamage;
        playerAttackSpeed = player.attackSpeed;
    }
    public void Flee()
    {
        fleePressed = true;
    }

    public void SkillFireBall()
    {
        playerTimer = 0;
        enemy.ReceiveDamage((int)playerAttackDamage * 2);

    }

    public void SkillFireRain()
    {
        playerTimer = -3;
        StartCoroutine(FireRain());
    }

    IEnumerator FireRain()
    {
        for(int i = 0; i<6; i++)
        {
            enemy.ReceiveDamage((int)(playerAttackDamage * 0.8f));
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
    }
}
