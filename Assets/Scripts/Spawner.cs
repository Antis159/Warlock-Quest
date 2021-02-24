using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PurpleSoul;
    private int enemyCount;
    public int mapHeight;
    public int mapWidth;
    private PlayerControl Player;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length + 1;
    }

    private void Update()
    {
        if (enemyCount <= 2)
            SpawnEnemy(PurpleSoul);
    }
    public void SpawnEnemy(GameObject enemy)
    {
        int x = Random.Range(-mapWidth, mapWidth);
        int y = Random.Range(-mapHeight, mapHeight);
        Vector3 spawnPos = new Vector3(x, y, 0);

        if (!Physics2D.OverlapCircle(new Vector2(x, y), 0.2f))
        {
            if (Player.gameObject.transform.position == spawnPos)
                return;

            GameObject spawnedEnemy = Instantiate(enemy, spawnPos, Quaternion.identity);
            spawnedEnemy.transform.parent = gameObject.transform;
            enemyCount++;
        }
        else
            return;
    }
    public void EnemyCountAdjust(int i)
    {
        enemyCount += i;
    }
}
