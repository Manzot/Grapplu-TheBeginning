using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBoundry : MonoBehaviour
{
    public int damage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.transform.GetComponent<EnemyUnit>().TakeDamage(damage);//.TakeDamage(100);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.GetComponent<PlayerController>().TakeDamage(damage);//.TakeDamage(100);
        }
    }
}
