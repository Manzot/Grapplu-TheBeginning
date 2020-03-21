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
        enemies.AddRange(GameObject.FindObjectsOfType<MeleeEnemy>());
        foreach (var enemy in enemies)
        {
            enemy.Initialize();
            enemy.transform.SetParent(parent);
        }
    }
    public void PostInitialize()
    {
        foreach (var enemy in enemies)
        {
            enemy.PostInitialize();
        }
    }
    public void Refresh()
    {
        foreach (var enemy in enemies)
        {
            enemy.Refresh();
        }
    }
    public void PhysicsRefresh()
    {
        foreach (var enemy in enemies)
        {
            enemy.PhysicsRefresh();
        }
    }

    public void Died(EnemyUnit e)
    {
        enemies.Remove(e);
    }
}
