using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : IManageables
{
    #region Singleton

    private static PlayerManager instance = null;

    public PlayerManager() { }

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerManager();
            }
            return instance;
        }
    }

    #endregion
    public void Initialize()
    {
        throw new System.NotImplementedException();
    }
    public void PhysicsRefresh()
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
}
