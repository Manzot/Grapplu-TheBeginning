using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType { Melee, Ranged, Flying};
public class EnemyUnit : MonoBehaviour, IDamage
{
    const float KNOCKAMOUNT = 200;
    const float MAX_GRAVITY = -12f;
    [HideInInspector]
   public const float TARGET_IN_RANGE = 200;

    float jumpCooldown = 1.5f;

    public EnemyType eType;

    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Collider2D coli;


    public float speed;
    public float jumpForce;
    public float attackCooldown;

    public int damage;
    public int hitPoints;
    public int currentHealth;
    public Image healthBar;
    GameObject healthBarParent;
    Quaternion defaultHbRotation;

    [HideInInspector]
    public bool isHurt;
    [HideInInspector]
    public bool isStunned;

    [HideInInspector]
    public const float STUN_TIME = .5f;
    [HideInInspector]
    public const int ASTAR_PATH_OFFSET = 0;

   [HideInInspector]
    public bool moveRight = true,
                isJumping,
                canAttack;
    [HideInInspector]
    public static bool targetFound;

    [HideInInspector]
    public float jumpTime = 0.5f,
                 attackCooldownTimer,
                 moveTimeCounter;

    public float moveTime;
    
    Collider2D groundCheckColi;

    [HideInInspector]
    public Transform target;
    public Transform feet;

    [HideInInspector]
    public AStarPathfinding aStar;
    [HideInInspector]
    public List<Node> aStarPath;
    [HideInInspector]
    public SetupWalkableArea walkable;

    Transform attackPosition;
    [HideInInspector]
   // public LineRenderer line;

    public virtual void Initialize()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coli = GetComponent<Collider2D>();
    }
    public virtual void PostInitialize()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;// FindObjectOfType<PlayerController>().transform;

        if(eType != EnemyType.Ranged)
        {
            attackPosition = transform.Find("AttackPos").transform;
        }

        walkable = FindObjectOfType<SetupWalkableArea>();
        attackCooldownTimer = Random.Range(attackCooldown - 1f, attackCooldown + 2f);
        speed = Random.Range(speed - 30f, speed + 35f);
        currentHealth = hitPoints;
        healthBar.fillAmount = currentHealth/hitPoints; 
        healthBarParent = healthBar.transform.parent.gameObject;
        defaultHbRotation = healthBarParent.transform.rotation;
        healthBarParent.SetActive(false);

        //line = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Line")).GetComponent<LineRenderer>();
        //aStar = new AStarPathfinding(walkable.walkAbleArea);
    }
    public virtual void Refresh()
    {
        if (healthBarParent.activeSelf)
        {
            healthBar.transform.rotation = defaultHbRotation;
        }
    }
    public virtual void PhysicsRefresh()
    {
        if(eType != EnemyType.Flying)
        {
            if (!Grounded())
            {
                if (rb.velocity.y < MAX_GRAVITY)
                {
                    rb.velocity = new Vector2(rb.velocity.x, MAX_GRAVITY);
                }
            }
        }
    }
    /// To check which direction the enemy is facing
    public void DirectionFacingWhenMoving()
    {
        if (rb.velocity.x > 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 0));
        else if (rb.velocity.x < 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 180));
    }
    public void LookingAtTarget()
    {
        if (target.position.x < transform.position.x)
            transform.rotation = Quaternion.Euler(new Vector2(0, 180));
        else
            transform.rotation = Quaternion.Euler(new Vector2(0, 0));

    }
    /// Check to see if the target is in range of enemy or not
    public bool FindTarget()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 8f);

        Collider2D playerCheck = Physics2D.OverlapCapsule(transform.position, new Vector2(1, 10), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Player"));
        if (playerCheck)
        {            
           // if ((hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")))
            {
                targetFound = true;
                aStar = new AStarPathfinding(walkable.walkAbleArea);
                aStarPath = aStar.FindPath(new Vector2Int((int)transform.position.x + ASTAR_PATH_OFFSET, (int)transform.position.y + ASTAR_PATH_OFFSET),
                    new Vector2Int((int)target.position.x + ASTAR_PATH_OFFSET, (int)target.position.y + ASTAR_PATH_OFFSET));
                return true;
            }
        }
        return false;
    }
    public bool FindTarget(Vector3 lookDirection, float range)
    {
      //  Debug.DrawRay(transform.position, new Vector3(transform.right.x, -1f, 0) * 6f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, range);
        
        if (hit.collider)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                targetFound = true;
                aStar = new AStarPathfinding(walkable.walkAbleArea);
                aStarPath = aStar.FindPath(new Vector2Int((int)transform.position.x + ASTAR_PATH_OFFSET, (int)transform.position.y + ASTAR_PATH_OFFSET),
                    new Vector2Int((int)target.position.x + ASTAR_PATH_OFFSET, (int)target.position.y + ASTAR_PATH_OFFSET));
                return true;
            }
        }
        return false;
    }
    /// To check if the enemy is on the ground or not
    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground","IObject","Platform"));
    }
    /// Jumping function
    public void Jump(Vector2 dir, Vector2 jumpPointer)
    {
        if (Physics2D.Raycast(transform.position, transform.up, 2.5f, LayerMask.GetMask("Ground", "Platform"))
        || Physics2D.Raycast(transform.position, transform.right, 1.3f, LayerMask.GetMask("Ground", "Platform")))
        {
            if (Grounded() && jumpTime < 0)
            {
                rb.AddForce(dir, ForceMode2D.Impulse);
                jumpTime = .7f;
                isJumping = true;
            }
        }

    }
    // jUMP FUNCTION WITH DIRECTION
    public void Jump2(Vector2 dir)
    {
        if (Grounded() && jumpTime < 0)
        {
            rb.AddForce(dir, ForceMode2D.Impulse);
            jumpTime = jumpCooldown;
            isJumping = true;
        }
    }
    //Enemy Death
    public bool Death()
    {
        if (currentHealth <= 0)
        {
            EnemyManager.Instance.Died(this.gameObject.GetComponent<EnemyUnit>());
            rb.velocity = Vector2.zero;
            anim.SetBool("isHurt", true);
            anim.SetBool("isDead", true);
            anim.SetTrigger("death");
            GameObject.Destroy(gameObject, 2.8f);
            return true;
        }
        return false;
    }
    /// Animation Function for disabling bools
    public void DisableBools(string boolName)
    {
        switch (boolName)
        {
            case "hurt":
                isHurt = false;// _boolean == "true";
                break;
            case "attack":
                canAttack = false;// _boolean == "true";
                break;
        }

    }

    public void TakeDamage(int damage)
    {
        if (!Death())
        {
            if (!healthBarParent.activeSelf)
            {
                healthBarParent.SetActive(true);
            }
            if (!isHurt)
            {
                isHurt = true;
                anim.SetTrigger("is_hurt");
                currentHealth -= damage;
                KnockBack();
            }
        }
        healthBar.fillAmount = (currentHealth) / (float)hitPoints; // Mathf.Lerp(currentHealth / (float)hitPoints, currentHealth - damage / (float)hitPoints, Time.deltaTime);//
    }
    // Enemy Hurt Function
    public void KnockBack()
    {
        rb.velocity = Vector2.zero;
        canAttack = false;
        Vector2 knockBckVector = (target.position - transform.position).normalized;
        rb.AddForce(knockBckVector * -KNOCKAMOUNT * Time.fixedDeltaTime, ForceMode2D.Impulse);
        TimerDelg.Instance.Add(() => { if (rb) { rb.velocity = new Vector2(0, rb.velocity.y); } }, 0.2f);
        TimerDelg.Instance.Add(() => { isHurt = false; }, STUN_TIME);
    }
    public void DamageTarget(float attackRange, int _damage)
    {
        RaycastHit2D hit = Physics2D.Raycast(attackPosition.position, transform.right, attackRange);

        if (hit.collider)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") || hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerSideFriction"))
            {
               target.GetComponent<PlayerController>().TakeDamage(_damage);
            }
        }
    }
}
