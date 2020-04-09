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
    Vector3 position;
    int SceneIndex;
    bool isLoaded = false;
    public void Initialize()
    {
        GameObject playerPrefab = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Karan/Player"));
        player = GameObject.FindObjectOfType<PlayerController>();

        if (!isLoaded)
        {
            player.transform.position = new Vector3(40,13);
        }
        else
        {
            player.transform.position = position;

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
        LoadGame();

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
    public void LoadGame()
    {
        playerData = new PlayerData(player);
        PlayerData playerdata = PlayerPersistence.LoadData();
       
            float x = playerdata.position[0];
            float y = playerdata.position[1];
            float z = 0;

            SceneIndex = playerData.sceneIndex;
            position = new Vector3(x, y, z);
            SceneManager.LoadScene(SceneIndex);
            /*player.transform.position = position;*/
            player.transform.rotation = Quaternion.Euler(Vector3.zero);
        isLoaded = true;
    }
}
