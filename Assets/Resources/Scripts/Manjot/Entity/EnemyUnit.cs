using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    const float KNOCKAMOUNT = 300;

    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;

    public float speed;
    public float jumpForce;
    public float attackCooldown;

    public int hitPoints;
    public int damage;

    [HideInInspector]
    public bool isHurt;
    [HideInInspector]
    public bool isStunned;

    [HideInInspector]
    public const float STUN_TIME = 0.5f;
    [HideInInspector]
    public const int ASTAR_PATH_OFFSET = 0;

   // [HideInInspector]
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

    public virtual void Initialize()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    public virtual void PostInitialize()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        walkable = FindObjectOfType<SetupWalkableArea>();
        attackCooldownTimer = Random.Range(attackCooldown - 1f, attackCooldown + 2f);
        //line = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Line")).GetComponent<LineRenderer>();
        //aStar = new AStarPathfinding(walkable.walkAbleArea);
    }
    public virtual void Refresh()
    {

    }
    public virtual void PhysicsRefresh()
    {

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 5f);
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
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground"));
    }
    /// Jumping function
    public void Jump(Vector2 dir)
    {
        if (Physics2D.Raycast(transform.position, transform.up, 1.3f, LayerMask.GetMask("Ground"))
        || Physics2D.Raycast(transform.position, transform.right, 1.3f, LayerMask.GetMask("Ground")))
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
            jumpTime = 1f;
            isJumping = true;
        }
    }
    // Damage Taking Function
    public void TakeDamage(int damage)
    {
        if(!isHurt && !isStunned)
        {
            hitPoints -= damage;
            isHurt = true;
        }
    }
    // Enemy Hurt Function
    public void Hurt()
    {
        canAttack = false;
        Vector2 knockBckVector = (target.position - transform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockBckVector * -KNOCKAMOUNT * Time.deltaTime, ForceMode2D.Impulse);
    }
    //Enemy Stunned Function
    public void Stunned()
    {
        isStunned = true;
        rb.velocity = Vector2.zero;
        TimerDelg.Instance.Add(() => { isStunned = false; }, STUN_TIME);
    }
    //Enemy Death
    public bool Death()
    {
        if (hitPoints <= 0)
        {
            EnemyManager.Instance.Died(this.gameObject.GetComponent<EnemyUnit>());
            GameObject.Destroy(gameObject, 2.8f);
            anim.SetBool("isHurt", true);

            anim.SetTrigger("death");
            rb.velocity = Vector2.zero;
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

}
