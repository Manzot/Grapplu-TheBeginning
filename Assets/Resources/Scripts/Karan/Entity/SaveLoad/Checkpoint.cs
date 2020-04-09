using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Checkpoint:MonoBehaviour
{
    PlayerData playerData;
    PlayerController player;

    void OnTriggerEnter2D(Collider2D colli)
    {
        if (colli.gameObject.CompareTag("Player"))
        {
            player = GameObject.FindObjectOfType<PlayerController>();
            playerData = new PlayerData();

            Debug.Log(colli.gameObject.name);
            playerData.Location = colli.gameObject.transform.position;
            Debug.Log(playerData.Location);
            /* playerData.Health = this.health;*/
            PlayerPersistence.SaveData(playerData);

        }
    }
}
