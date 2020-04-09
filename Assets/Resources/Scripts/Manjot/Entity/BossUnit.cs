using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossUnit : MonoBehaviour
{

    public float speed;
    public float jumpForce;
    public float hitPoints;
    public float attackCooldown;
    public bool enraged;
    public bool isHurt;
    public bool isAttacking;
    public int damage = 15;


    const float MAX_GRAVITY = -12;

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

    bool shaderChange;

    GameObject bossHealthUI;
    Image healthBar;

    public virtual void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        bossHealthUI = GameObject.FindGameObjectWithTag("BossUI");
        bossHealthUI.transform.GetChild(0).gameObject.SetActive(true);
        bossHealthUI.transform.GetChild(1).gameObject.SetActive(true);
        healthBar = bossHealthUI.transform.GetChild(1).GetComponent<Image>();
        healthBar.fillAmount = hitPoints/hitPoints;
        target = GameObject.FindGameObjectWithTag("Player").transform;//FindObjectOfType<PlayerController>().transform;
    }
    public virtual void PostInitialize()
    {
        defaultColor = sprite.material.color;
        currentHealth = hitPoints;
    }

    public virtual void Refresh()
    {
      //  Debug.Log(currentHealth);
        if (currentHealth < hitPoints / 2)
        {
            enraged = true;
            anim.SetBool("enraged", true);
        }

        if (shaderChange)
        {
            sprite.material.color = Color.red;
            TimerDelg.Instance.Add(() => { shaderChange = false; }, .3f);
        }
        else
        {
            sprite.material.color = defaultColor;
        }
    }
    public virtual void PhysicsRefresh()
    {
        if (rb.velocity.y < MAX_GRAVITY)
        {
            rb.velocity = new Vector2(rb.velocity.x, MAX_GRAVITY);
        }
    }
    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground"));
    }

    public void LookingAtTarget()
    {
       // if (!isAttacking)
        {
            if (target.position.x < transform.position.x)
                transform.rotation = Quaternion.Euler(new Vector2(0, 180));
            else
                transform.rotation = Quaternion.Euler(new Vector2(0, 0));
        }
    }

    public void TakeDamage(int damage)
    {
        if (!Dead())
        {
            if (!isHurt)
            {
                isHurt = true;
                shaderChange = true;
                currentHealth -= damage;
                rb.velocity = Vector2.zero;
                TimerDelg.Instance.Add(()=> { isHurt = false; }, 1f);
            }
            healthBar.fillAmount = currentHealth / (float)hitPoints;
        }
    }

    public bool Dead()
    {
        if(currentHealth <= 0)
        {
            anim.SetTrigger("death");
            bossHealthUI.transform.GetChild(0).gameObject.SetActive(false);
            bossHealthUI.transform.GetChild(1).gameObject.SetActive(false);
            Destroy(gameObject, 2f);
            BossManager.Instance.Died(this);
            return true;
        }
        return false;
    }
}
