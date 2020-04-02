using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttacks : ThrowAbles
{
    GameObject fireBallFX;
    public int damage = 10;
    public float lifeTime = 5f;

    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        fireBallFX = transform.GetChild(0).gameObject;
        Vector2 dir = (target.position - transform.position).normalized;
        var angle2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle2, Vector3.forward);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Throwable"), LayerMask.NameToLayer("Enemy"), true);
        TimerDelg.Instance.Add(() => { Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Throwable"), LayerMask.NameToLayer("Enemy"), false); }, 1.5f);
        Destroy(gameObject, lifeTime);
    }

    public void Throw()
    {
        anim.SetTrigger("throw");
        fireBallFX.gameObject.SetActive(true);
        transform.parent = null;
        rb.AddForce(transform.right * throwSpeed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            }
            else if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.gameObject.GetComponent<EnemyUnit>().TakeDamage(damage);
            }
            else if(collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                collision.gameObject.GetComponent<BossUnit>().TakeDamage(damage);
            }
        }
    }
}
