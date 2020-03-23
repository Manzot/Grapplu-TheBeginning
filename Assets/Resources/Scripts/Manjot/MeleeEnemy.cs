using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    const float STOPPING_DISTANCE = 0.3f;
    const int ASTAR_PATH_OFFSET = 0;
    const float STUN_TIME = 0.4f;

    bool moveRight = true;
    bool targetFound;
    bool isJumping;
    bool canAttack;
    public bool isHurt;
    public bool isStunned;

    float jumpTime = 0.5f;
    float moveTimeCounter;
    public float moveTime;

    EnemyTypes eType;
    Collider2D groundCheckColi;
    Transform target;
    public Transform feet;

    AStarPathfinding aStar;
    List<Node> aStarPath;
    SetupWalkableArea walkable;
   // LineRenderer line;

    /// Awake Function
    public override void Initialize()
    {
        base.Initialize();
    }
    //// Start Function
    public override void PostInitialize()
    {
        base.PostInitialize();
        eType = EnemyTypes.Melee;
        target = GameObject.FindGameObjectWithTag("Player").transform;
      //line = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Line")).GetComponent<LineRenderer>();
        walkable = FindObjectOfType<SetupWalkableArea>();
      //aStar = new AStarPathfinding(walkable.walkAbleArea);
    }
    /// Update Function
    public override void Refresh()
    {
        if (Input.GetKeyDown(KeyCode.K))
            isHurt = true;

        if (!Death())
        {
            Hurt();
            Timers();
            AnimationsCaller();

            if (!isHurt && !isStunned)
            {
                DirectionFacingWhenMoving();
                if (!targetFound) // Searching for target
                {
                    RandomMove();
                    FindTarget();
                }
                else // When target is found
                {
                    if (!canAttack && !isJumping)
                    {
                        if (target.position.y > transform.position.y + 1f)
                        {
                            FollowWithAstar();
                        }
                        else
                        {
                            FollowPlayer();
                        }
                    }

                    if (Vector2.SqrMagnitude(new Vector2(transform.position.x - target.position.x, 0)) < STOPPING_DISTANCE &&
                        Vector2.SqrMagnitude(new Vector2(transform.position.y - target.position.y, 0)) < STOPPING_DISTANCE)
                    {
                        canAttack = true;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        DirectionFacingAtCloseDistance();
                    }
                }
            }
        }
    }

    //// Functions////////////////

    /// To check which direction the enemy is facing
    void DirectionFacingWhenMoving()
    {
        if (rb.velocity.x > 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 0));
        else if (rb.velocity.x < 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 180));
    }
    void DirectionFacingAtCloseDistance()
    {
        if (target.position.x < transform.position.x)
            transform.rotation = Quaternion.Euler(new Vector2(0, 180));
        else
            transform.rotation = Quaternion.Euler(new Vector2(0, 0));

    }
    /// Random movement of enemies to find player
    void RandomMove()
    {
        if (moveTimeCounter <= 0)
        {
            moveRight = !moveRight;
            moveTimeCounter = moveTime;
        }
        if (moveRight) rb.velocity = new Vector2(1 * speed * Time.fixedDeltaTime, rb.velocity.y); // Move Right
        else rb.velocity = new Vector2(-1 * speed * Time.fixedDeltaTime, rb.velocity.y); // Move Left
    }
    /// Check to see if the target is in range of enemy or not
    bool FindTarget()
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
    /// Following target 
    void FollowPlayer()
    {
        if (target.position.y < transform.position.y - .5f)
        {
            rb.velocity = (new Vector2(transform.right.x * speed * Time.deltaTime, rb.velocity.y));
        }
        else
            rb.velocity = new Vector2((target.position.x - transform.position.x), 0).normalized * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);

    }
    void FollowWithAstar()
    {
        aStar = new AStarPathfinding(walkable.walkAbleArea);
        aStarPath = aStar.FindPath(new Vector2Int((int)transform.position.x + ASTAR_PATH_OFFSET, (int)transform.position.y + ASTAR_PATH_OFFSET),
                new Vector2Int((int)target.position.x + ASTAR_PATH_OFFSET, (int)target.position.y + ASTAR_PATH_OFFSET));

        //DrawLine(aStarPath);

        if (aStarPath.Count > 0)
        {
            Vector2 newPath = new Vector2(aStarPath[1].position.x - aStarPath[0].position.x, 0).normalized;
            rb.velocity = newPath * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);

            if (aStarPath[1].position.y > aStarPath[0].position.y)
                Jump();
        }
    }
    /// Jumping function
    void Jump()
    {
        if (Physics2D.Raycast(transform.position, transform.up, 1.3f, LayerMask.GetMask("Ground"))
               || Physics2D.Raycast(transform.position, transform.right, 1.3f, LayerMask.GetMask("Ground")))
        {
            if (Grounded() && jumpTime < 0)
            {
                rb.AddForce(new Vector2(rb.velocity.x, jumpForce) * Time.fixedDeltaTime, ForceMode2D.Impulse);
                jumpTime = .7f;
                isJumping = true;
            }
        }

    }
    /// To check if the enemy is on the ground or not
    bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground"));
    }

    /// Atttacking player
    void AnimationsCaller()
    {
        anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));

        if (canAttack)
            anim.SetBool("isAttacking", true);
        else
            anim.SetBool("isAttacking", false);
        
    }
    // All the Timers
    void Timers()
    {

        moveTimeCounter -= Time.deltaTime;
        jumpTime -= Time.deltaTime;

        if (jumpTime < 0)
        {
            isJumping = false;
        }
    }

    void Hurt()
    {
        if (isHurt)
        {
            canAttack = false;
            Vector2 knockBckVector = (target.position - transform.position).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(knockBckVector * -160 * Time.deltaTime , ForceMode2D.Impulse);
            anim.SetBool("isHurt", true);
        }
        else
        {
            anim.SetBool("isHurt", false);
        }
    }
    public void Stunned()
    {
        isStunned = true;
        rb.velocity = Vector2.zero;
        TimerDelg.Instance.Add(() => { isStunned = false; }, STUN_TIME);
    }

    bool Death()
    {
        if(hitPoints <= 0)
        {
            rb.velocity = Vector2.zero;
            EnemyManager.Instance.Died(this.gameObject.GetComponent<EnemyUnit>());
            GameObject.Destroy(gameObject, 2.8f);
            anim.SetTrigger("death");
            return true;
        }
        return false;
    }
    /// Animation Functions
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
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Drawing gizmos
    /// Check if there is any platform to jump
    void DrawRaysInScene()
    {
        if (targetFound)
        {
            Debug.DrawRay(transform.position, transform.up * 1.3f, Color.red);
            Debug.DrawRay(transform.position, transform.right * 1.3f, Color.red);
        }
        else
            Debug.DrawRay(transform.position, transform.right * 5, Color.red);
    }

    /// Drawing Line that shows walking path
    //void DrawLine(List<Node> path)
    //{
    //    line.positionCount = path.Count;

    //    Vector3[] linePoints = new Vector3[path.Count];
    //    for (int i = 0; i < path.Count; i++)
    //    {
    //        linePoints[i] = new Vector3(path[i].position.x, path[i].position.y, 0);
    //    }
    //    line.SetPositions(linePoints);
    //}
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(new Vector2(feet.position.x, feet.position.y), .2f);
    //}

}
