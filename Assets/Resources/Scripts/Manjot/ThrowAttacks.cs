using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttacks : ThrowAbles
{
    public int damage = 10;
    public float lifeTime = 5f;
    [HideInInspector]
    public bool isTriggered;
    
    GameObject laserExplosionGO;
    public bool isLaser = false;
    [HideInInspector]
    public SpriteRenderer spriteRend;
    public void Initialize()
    {

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (target) { 
        Vector2 dir = (target.position - transform.position).normalized;
            var angle2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle2, Vector3.forward);
            
        }
        spriteRend = GetComponent<SpriteRenderer>();


        if (isLaser)
        {
            laserExplosionGO = transform.GetChild(0).gameObject;
            laserExplosionGO.SetActive(false);
        }

        Destroy(gameObject, lifeTime);
    }

    public void Throw(float speedModifier = 1)
    {
        if (!isLaser)
        {
            anim.SetTrigger("throw");
        }
        transform.parent = null;
        rb.AddForce(transform.right * throwSpeed * speedModifier, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.layer != LayerMask.NameToLayer("Platform"))
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
                DestryWithExplosion();
                if (isLaser)
                {
                    Destroy(gameObject);
                }
            }
            if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.gameObject.GetComponent<EnemyUnit>().TakeDamage(damage);
                DestryWithExplosion();
            }
            if(collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                collision.gameObject.GetComponent<BossUnit>().TakeDamage(damage);
                DestryWithExplosion();
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                DestryWithExplosion();
                if (isLaser)
                {
                    Destroy(gameObject, 0.5f);
                }
            }

        }
    }

    void DestryWithExplosion()
    {
        isTriggered = true;
        rb.velocity = Vector2.zero;
        if(!isLaser)
        {
            anim.SetTrigger("explode");
            Destroy(gameObject, 1.5f);
        }
           
    }
}
