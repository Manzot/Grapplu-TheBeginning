using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUnit : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float hitPoints;
    public float attackCooldown;
    public bool enraged;
    public bool isHurt;

    [HideInInspector]
    public Collider2D groundCheckColi;
    [HideInInspector]
    public Transform target;

    public Transform feet;

    //[HideInInspector]
    public float currentHealth;

    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;

    public virtual void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = FindObjectOfType<PlayerController>().transform;
    }
    public virtual void PostInitialize()
    {
        currentHealth = hitPoints;
    }

    public virtual void Refresh()
    {

    }
    public virtual void PhysicsRefresh()
    {
       
    }
    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground"));
    }

    public void TakeDamage(int damage)
    {
        if (!isHurt)
        {
            currentHealth -= damage;
            //anim.SetBool("isHurt", true);
        }
    }

    public bool Dead()
    {
        if(currentHealth <= 0)
        {
            //anim.SetBool("isHurt", true);
            //anim.SetTrigger("is_hurt");
            anim.SetTrigger("death");
            Destroy(gameObject, 2f);
            BossManager.Instance.Died(this);
            return true;
        }
        return false;
    }
}
