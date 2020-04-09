using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttacks : ThrowAbles
{
   // GameObject fireBallFX;
    public int damage = 10;
    public float lifeTime = 5f;

    public void Initialize()
    {
        if (FindObjectOfType<PlayerController>())
        {
            target = FindObjectOfType<PlayerController>().transform;
        }
        else
        {
            target = this.transform;
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
       // fireBallFX = transform.GetChild(0).gameObject;
        Vector2 dir = (target.position - transform.position).normalized;
        var angle2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle2, Vector3.forward);
        Destroy(gameObject, lifeTime);
    }

    public void Throw()
    {
        anim.SetTrigger("throw");
       // fireBallFX.gameObject.SetActive(true);
        transform.parent = null;
        rb.AddForce(transform.right * throwSpeed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.layer != LayerMask.NameToLayer("Platform"))
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
                Destroy(gameObject);
            }
            if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.gameObject.GetComponent<EnemyUnit>().TakeDamage(damage);
                Destroy(gameObject);
            }
            if(collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                collision.gameObject.GetComponent<BossUnit>().TakeDamage(damage);
                Destroy(gameObject);
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
