using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticleMove : MonoBehaviour
{
    private Vector3 move;
    private Enemy enemy;
    private int damageToDeal;
    void Update()
    {
        if(move != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, move, 5f * Time.deltaTime);
        }
        if (transform.position == move)
        {
            if (enemy != null)
                enemy.ReceiveDamage(damageToDeal);
            Destroy(gameObject);
        }
    }

    public void SetMove(Vector3 enemyPos)
    {
        move = enemyPos;
    }

    public void SetEnemy(Enemy lastEnemy)
    {
        enemy = lastEnemy;
    }
    public void SetDamage(int damage)
    {
        damageToDeal = damage;
    }
}
