using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : IManageables
{
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
        throw new System.NotImplementedException();
    }
    public void PostInitialize()
    {
        throw new System.NotImplementedException();
    }
    public void Refresh()
    {
        throw new System.NotImplementedException();
    }
    public void PhysicsRefresh()
    {
        throw new System.NotImplementedException();
    }
}
