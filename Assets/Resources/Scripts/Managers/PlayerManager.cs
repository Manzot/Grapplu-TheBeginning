using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : IManageables
{
    PlayerController player;
    private float spawnTime = 5f;
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
    PlayerData playerData;
    bool isLoaded = false;
    public void Initialize()
    {
        GameObject playerPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Karan/Player"));
        player = GameObject.FindObjectOfType<PlayerController>();

        if (!SaveLoadManager.isLoaded)
        {
            player.transform.position = new Vector3(10,13);
        }
        else
        {
            player.transform.position = SaveLoadManager.Instance.position;
        }

        player.isAlive = true;
        player.Initialize();
    }
    public void PhysicsRefresh()
    {
        if (player.isAlive)
            player.PhysicsRefresh();
    }
    public void PostInitialize()
    {
        player.PostInitialize();
    }
    public void Refresh()
    {
        if (player.isAlive)
            player.Refresh();
        IsDead();

    }
    public void PlayerSpawn()
    {
        player.health = 100f;
        player.Initialize();
        player.gameObject.SetActive(true);
        SaveLoadManager.Instance.Load();
        isLoaded = true;    

       /* playerData = PlayerPersistence.LoadData(player);
        player.transform.position = playerData.Location;*/
       
       
          
    }
    public void IsDead()
    {
        if (!player.isAlive)
        {
            Vector3 deathLoc = player.deathLoc;
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0)
            {
                PlayerSpawn();
              /*  player.health = player.playerData.Health;*/
                spawnTime = 5f;
            }
        }
    }
}
