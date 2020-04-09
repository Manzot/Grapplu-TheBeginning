using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Checkpoint:MonoBehaviour
{
    PlayerData playerData;
    PlayerController player;
  
    void OnTriggerEnter2D(Collider2D colli)
    {
        if (colli.gameObject.CompareTag("Player"))
        {
            
            player = GameObject.FindObjectOfType<PlayerController>();
            playerData = new PlayerData(player);
            player.savePoint.x = colli.gameObject.transform.position.x;
            player.savePoint.y = colli.gameObject.transform.position.y;
            player.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            PlayerPersistence.SaveData(player);
            
            /*
            playerData = new PlayerData();

            Debug.Log(colli.gameObject.name);
            Debug.Log(playerData.Location);
             playerData.Health = this.health;
            PlayerPersistence.SaveData(player);*/

        }
    }
}
