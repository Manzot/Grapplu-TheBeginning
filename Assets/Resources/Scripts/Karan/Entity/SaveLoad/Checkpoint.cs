using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Checkpoint:MonoBehaviour
{
    PlayerData playerData;
    PlayerController player;
    public static Vector3 location;
    public static int currentSceneIndex;


    public void Awake()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex; 
    }
    void OnTriggerEnter2D(Collider2D colli)
    {
        if (colli.gameObject.CompareTag("Player"))
        {
           
            location = colli.gameObject.transform.position;
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SaveLoadManager.Instance.Save();
            /*Debug.Log(location);*/
            // player.savePoint.x = colli.gameObject.transform.position.x;
            // player.savePoint.y = colli.gameObject.transform.position.y;
            // player.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            // PlayerPersistence.SaveData(player);

            /*
            playerData = new PlayerData();

            Debug.Log(colli.gameObject.name);
            Debug.Log(playerData.Location);
             playerData.Health = this.health;
            PlayerPersistence.SaveData(player);*/

        }
        
    }
}
