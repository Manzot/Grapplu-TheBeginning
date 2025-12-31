using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : IManageables
{
    
    Transform parent;
    public List<BossUnit> bosses;
    public BossUnit demonBoss;
    public BossUnit elementalBoss;

    #region Singleton

    private static BossManager instance = null;

    public BossManager() { }
    
    public static BossManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BossManager();
            }
            return instance;
        }
    }
    #endregion
    public void Initialize()
    {
        demonBoss = Resources.Load<GameObject>("Prefabs/Manjot/Bosses/Demon_Boss").transform.GetComponentInChildren<BossUnit>();
        parent = new GameObject("BossParent").transform;
        bosses = new List<BossUnit>();
        bosses.AddRange(GameObject.FindObjectsOfType<BossUnit>());

        for (int i = 0; i < bosses.Count; i++)
        {
            bosses[i].Initialize();
        }
    }

    public void PostInitialize()
    {
        for (int i = 0; i < bosses.Count; i++)
        {
            bosses[i].PostInitialize();
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < bosses.Count; i++)
        {
            bosses[i].Refresh();
        }
    }
    public void PhysicsRefresh()
    {
        for (int i = 0; i < bosses.Count; i++)
        {
            bosses[i].PhysicsRefresh();
        }
    }

    public BossUnit SpawnBoss(BossUnit boss, Vector2 pos)
    {
        boss = GameObject.Instantiate(boss, pos, Quaternion.identity, parent);
        bosses.Add(boss);
        boss.Initialize();
        boss.PostInitialize();
        return boss;
    }

    public void Died(BossUnit boss)
    {
        bosses.Remove(boss);
    }

}
