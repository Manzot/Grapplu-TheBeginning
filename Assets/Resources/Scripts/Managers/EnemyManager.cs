using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypes { Melee, Ranged, Flying, Underground }
public class EnemyManager : IManageables
{
    Dictionary<EnemyType, GameObject> enemiesPrefabs;
    public List<EnemyUnit> enemiesList;
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
        enemiesPrefabs = new Dictionary<EnemyType, GameObject>();
        enemiesPrefabs.Add(EnemyType.Melee, Resources.Load<GameObject>("Prefabs/Manjot/Enemies/MeleeEnemy"));
        enemiesPrefabs.Add(EnemyType.Ranged, Resources.Load<GameObject>("Prefabs/Manjot/Enemies/RangedEnemy"));
        enemiesPrefabs.Add(EnemyType.Flying, Resources.Load<GameObject>("Prefabs/Manjot/Enemies/FlyingEnemy"));

        parent = new GameObject("EnemiesParent").transform;
        enemiesList = new List<EnemyUnit>();

        enemiesList.AddRange(GameObject.FindObjectsOfType<EnemyUnit>());
        for (int i = 0; i < enemiesList.Count; i++)
        {
            enemiesList[i].Initialize();
            if (enemiesList[i].transform.parent == null)
                enemiesList[i].transform.SetParent(parent);
            else
                enemiesList[i].transform.parent.SetParent(parent);
        }
    }
    public void PostInitialize()
    {
       
        for (int i = 0; i < enemiesList.Count; i++)
        {
            enemiesList[i].PostInitialize();
        }
    }
    public void Refresh()
    {

        for (int i = 0; i < enemiesList.Count; i++)
        {
            enemiesList[i].Refresh();
        }
    }
    public void PhysicsRefresh()
    {
        
        for (int i = 0; i < enemiesList.Count; i++)
        {
            enemiesList[i].PhysicsRefresh();
        }
    }

    public void Died(EnemyUnit e)
    {
        enemiesList.Remove(e);
    }

    public EnemyUnit SpawnEnemy(EnemyType eType, Vector2 pos)
    {
        EnemyUnit newEnemy;
        if(eType == EnemyType.Flying)
        {
            newEnemy = GameObject.Instantiate(enemiesPrefabs[eType], pos, Quaternion.identity, parent).transform.GetComponentInChildren<EnemyUnit>();
        }
        else
        {
            newEnemy =  GameObject.Instantiate(enemiesPrefabs[eType], pos, Quaternion.identity, parent).GetComponent<EnemyUnit>();
        }
        newEnemy.Initialize();
        newEnemy.PostInitialize();
        enemiesList.Add(newEnemy);

        return newEnemy;
    }
}
