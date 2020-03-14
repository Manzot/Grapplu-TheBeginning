using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : IManageables
{
    List<MeleeEnemy> meleeEnemies;
    #region Singleton

    private static EnemyManager instance = null;

    public EnemyManager() { }

    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemyManager();
            }
            return instance;
        }
    }

    #endregion
    public void Initialize()
    {
        meleeEnemies = new List<MeleeEnemy>();
        meleeEnemies.AddRange(GameObject.FindObjectsOfType<MeleeEnemy>());
        foreach (var meleeE in meleeEnemies)
        {
            meleeE.Initialize();
        }
    }
    public void PostInitialize()
    {
        foreach (var meleeE in meleeEnemies)
        {
            meleeE.PostInitialize();
        }
    }
    public void Refresh()
    {
        foreach (var meleeE in meleeEnemies)
        {
            meleeE.Refresh();
        }
    }
    public void PhysicsRefresh()
    {
        foreach (var meleeE in meleeEnemies)
        {
            meleeE.PhysicsRefresh();
        }
    }
}
