using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveLoadManager
{
    #region Singleton
    private static SaveLoadManager instance;
    static SaveLoadManager() { }
    public static SaveLoadManager Instance { get { return instance ?? (instance = new SaveLoadManager()); } }
    #endregion

    PlayerController player;
    PlayerData playerData;
    Checkpoint checkpoint;
    public Vector3 position;
    static int sceneIndex;
    bool isLoaded;
    public void Initialize()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        playerData = new PlayerData(player);
        checkpoint = GameObject.FindObjectOfType<Checkpoint>();
       
    }
    public void Save() {

        /*playerData = new PlayerData(player);
        */
        position = Checkpoint.location;
        player.savePoint = position;
        /* Debug.Log(position);*/
        if (!checkpoint)
        {
            player.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
        else
        {
            player.currentSceneIndex = Checkpoint.currentSceneIndex;
        }
        PlayerPersistence.SaveData(player);
    }
   
    public void Load()
    {
       /* playerData = new PlayerData(player);*/
        PlayerData playerdata = PlayerPersistence.LoadData();

        float x = playerdata.position[0];
        float y = playerdata.position[1];
        float z = 0;
        
        sceneIndex = playerdata.sceneIndex;
        position = new Vector3(x, y, z);
        SceneManager.LoadScene(sceneIndex);
        /*player.transform.position = position;*/
        player.transform.rotation = Quaternion.Euler(Vector3.zero);
        isLoaded = true;
    }

}
