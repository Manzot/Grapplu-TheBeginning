using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject wall1, wall2, bossSpawnLocation;
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                BossManager.Instance.SpawnBoss(BossManager.Instance.demonBoss, bossSpawnLocation.transform.position);
                wall1.gameObject.SetActive(true);
                wall2.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
