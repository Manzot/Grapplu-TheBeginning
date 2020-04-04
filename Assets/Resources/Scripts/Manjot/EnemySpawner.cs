﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool ToSpawnBossesEnableThis;

    public GameObject bossToSpawn;
    public Transform bossLocation;

    public GameObject colliderZone;

    public int numOfMeleeEnemies;
    public int numOfRangedEnemies;
    public int numOfFlyingEnemies;
    public int numOfWaves;

    public Transform[] positionsMelee;
    public Transform[] positionsRanged;
    public Transform[] positionsFlying;

    int enemyID = 0;
    bool waveSpawned;
    bool isTriggered;

    EnemyUnit[] newEnemies;
    List<EnemyUnit> enemieCountList;
    List<BossUnit> bossCountList;

    public void PostInitialize()
    {
        colliderZone.SetActive(false);
        enemieCountList = new List<EnemyUnit>();
        bossCountList = new List<BossUnit>();
        newEnemies = new EnemyUnit[numOfRangedEnemies + numOfMeleeEnemies + numOfFlyingEnemies];
    }
    public void Refresh()
    {
        if (isTriggered)
        {
            colliderZone.SetActive(true);

            if (ToSpawnBossesEnableThis)
            {
                if (isTriggered)
                {
                    if (bossCountList[0].currentHealth <= 0)
                    {
                        bossCountList.Remove(bossCountList[0]);
                        EnemySpawnerManager.Instance.enemySpawnersList.Remove(this);
                        Destroy(gameObject, 1f);
                    }
                }
            }
            else
            {
                if (waveSpawned)
                {
                    if (numOfWaves > 0)
                    {
                        if (enemieCountList.Count <= 0)
                        {
                            enemyID = 0;
                            Spawner();
                        }
                    }
                    else
                    {
                        if (enemieCountList.Count <= 0)
                        {
                            EnemySpawnerManager.Instance.enemySpawnersList.Remove(this);
                            Destroy(gameObject, 1f);
                        }
                    }
                }
                for (int i = 0; i < enemieCountList.Count; i++)
                {
                    if (enemieCountList[i].currentHealth <= 0)
                    {
                        enemieCountList.Remove(enemieCountList[i]);
                    }
                }
            }
        }

        
       
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered)
        {
                if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (ToSpawnBossesEnableThis)
                    {
                        BossUnit boss = BossManager.Instance.SpawnBoss(bossToSpawn.GetComponent<BossUnit>(), bossLocation.position);
                        bossCountList.Add(boss);
                        isTriggered = true;
                    }
                    else
                    {
                        Spawner();
                        isTriggered = true;
                    }
                }
           
        }
    }
    void Spawner()
    {
        numOfWaves--;
        for (int i = 0; i < numOfMeleeEnemies; i++)
        {
            newEnemies[enemyID] = EnemyManager.Instance.SpawnEnemy(EnemyType.Melee, positionsMelee[0].position);
            enemieCountList.Add(newEnemies[enemyID]);
            enemyID++;
        }
        for (int i = 0; i < numOfRangedEnemies; i++)
        {
            newEnemies[enemyID] = EnemyManager.Instance.SpawnEnemy(EnemyType.Ranged, positionsRanged[0].position);
            enemieCountList.Add(newEnemies[enemyID]);
            enemyID++;
        }
        for (int i = 0; i < numOfFlyingEnemies; i++)
        {
            newEnemies[enemyID] = EnemyManager.Instance.SpawnEnemy(EnemyType.Flying, positionsFlying[0].position);
            enemieCountList.Add(newEnemies[enemyID]);
            enemyID++;
        }
        waveSpawned = true;
    }
}