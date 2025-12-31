using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : IManageables
{
    public List<EnemySpawner> enemySpawnersList;

    #region Singleton
    private static EnemySpawnerManager instance = null;

    public EnemySpawnerManager() { }

    public static EnemySpawnerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemySpawnerManager();
            }
            return instance;
        }
    }
    #endregion

    public void Initialize()
    {
        //throw new System.NotImplementedException();
        enemySpawnersList = new List<EnemySpawner>();
    }

    public void PostInitialize()
    {
        enemySpawnersList.AddRange(GameObject.FindObjectsOfType<EnemySpawner>());
        for (int i = 0; i < enemySpawnersList.Count; i++)
        {
            enemySpawnersList[i].PostInitialize();
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < enemySpawnersList.Count; i++)
        {
            enemySpawnersList[i].Refresh();
        }
    }

    public void PhysicsRefresh()
    {
        //throw new System.NotImplementedException();
    }



}
