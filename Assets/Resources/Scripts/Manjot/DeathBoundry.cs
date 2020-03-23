using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBoundry : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.transform.GetComponent<EnemyUnit>().TakeDamage(100);//.TakeDamage(100);
        }
    }
}
