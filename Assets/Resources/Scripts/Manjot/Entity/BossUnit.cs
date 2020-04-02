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
    public bool isAttacking;


    const float maxGravity = -12;

    [HideInInspector]
    public Collider2D groundCheckColi;
    [HideInInspector]
    public Transform target;

    public Transform feet;

    //[HideInInspector]
    public float currentHealth;

    SpriteRenderer sprite;
    Color defaultColor;

    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;

    public virtual void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        target = FindObjectOfType<PlayerController>().transform;
    }
    public virtual void PostInitialize()
    {
        defaultColor = sprite.material.color;
        currentHealth = hitPoints;
    }

    public virtual void Refresh()
    {
        if (currentHealth < hitPoints / 2)
        {
            enraged = true;
            anim.SetBool("enraged", true);
        }

        if (isHurt)
        {
            sprite.material.color = Color.red;
        }
        else
        {
            sprite.material.color = defaultColor;
        }
    }
    public virtual void PhysicsRefresh()
    {
        if (rb.velocity.y < maxGravity)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxGravity);
        }
    }
    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground"));
    }

    public void TakeDamage(int damage)
    {
        if (!isHurt)
        {
            isHurt = true;
            currentHealth -= damage;
           // anim.SetTrigger("is_hurt");
            rb.velocity = Vector2.zero;
            TimerDelg.Instance.Add(()=> { isHurt = false; }, .2f);
        }
    }

    public bool Dead()
    {
        if(currentHealth <= 0)
        {
            anim.SetTrigger("death");
            Destroy(gameObject, 2f);
            BossManager.Instance.Died(this);
            return true;
        }
        return false;
    }
}
