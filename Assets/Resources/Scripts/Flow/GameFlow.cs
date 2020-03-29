using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow: IManageables
{
    #region Singleton

    private static GameFlow instance = null;

    public GameFlow() { }

    public static GameFlow Instance {
        get
        {
            if (instance == null)
            {
                instance = new GameFlow();
            }
            return instance;
        }
    }

    #endregion

    public void Initialize()
    {
        EnemyManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
    }
    public void PostInitialize()
    {
        TimerDelg.Instance.PostInitialize();
        EnemyManager.Instance.PostInitialize();
        PlayerManager.Instance.PostInitialize();
    }
    public void Refresh()
    {
        TimerDelg.Instance.Refresh();
        EnemyManager.Instance.Refresh();
        PlayerManager.Instance.Refresh();
    }
    public void PhysicsRefresh()
    {
        EnemyManager.Instance.PhysicsRefresh();
        PlayerManager.Instance.PhysicsRefresh();
    }

}
