using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypes { Melee, Ranged, Flying, Underground }
public class EnemyManager : IManageables
{
    List<EnemyUnit> enemies;
    Transform parent;
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
        parent = new GameObject("EnemiesParent").transform;
        enemies = new List<EnemyUnit>();
        enemies.AddRange(GameObject.FindObjectsOfType<EnemyUnit>());
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Initialize();
            if (enemies[i].transform.parent == null)
                enemies[i].transform.SetParent(parent);
            else
                enemies[i].transform.parent.SetParent(parent);
        }
    }
    public void PostInitialize()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].PostInitialize();
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Refresh();
        }
    }
    public void PhysicsRefresh()
    {
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].PhysicsRefresh();
        }
    }

    public void Died(EnemyUnit e)
    {
        enemies.Remove(e);
    }
}
