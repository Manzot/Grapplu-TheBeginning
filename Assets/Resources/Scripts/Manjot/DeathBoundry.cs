using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBoundry : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.transform.GetComponent<EnemyUnit>().TakeDamage(1000);//.TakeDamage(100);
        }
    }
}
