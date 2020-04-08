using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    const float ATTACKING_DISTANCE = 0.4f;
    const float STOPPING_DISTANCE = 7f;
    const float ATTACK_MOVE_TIME = 1f;

    public Transform jumpPointer;
    
    float attackMoveTimer;
    bool attackMoveLR;
    
    // LineRenderer line;

    /// Awake Function
    public override void Initialize()
    {
        base.Initialize();
        eType = EnemyType.Melee;
    }
    //// Start Function
    public override void PostInitialize()
    {
        base.PostInitialize();

    }
    /// Update Function
    public override void Refresh()
    {
        base.Refresh();

        if (!Death())
        {
            AnimationCaller();
            Timers();

            if (!isHurt)
            {
                if (!targetFound) // Searching for target
                {
                    DirectionFacingWhenMoving();
                    FindTarget();
                }
            }
        }
    }

    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();

        if (!Death())
        {
            if (!isHurt)
            {
                if (!targetFound) 
                {
                    MoveLeftRight();
                }
                else // When target is found
                {
                    TargetFollowFunctionFull();
                    AtackMove();
                }
            }
        }

    }
    //FollowTarget FInal Complete Version
    void TargetFollowFunctionFull()
    {
        DirectionFacingWhenMoving();
        if (!canAttack && !isJumping)
        {
            if ((transform.position - target.position).sqrMagnitude <= TARGET_IN_RANGE - 80)
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
            else
                MoveLeftRight();

        }

    }
    /// Following target 
    void FollowPlayer()
    {
        DirectionFacingWhenMoving();
        if (target.position.y < transform.position.y - .5f)
        {
            rb.velocity = (new Vector2(transform.right.x * speed * Time.fixedDeltaTime, rb.velocity.y));
        }
        else
            rb.velocity = new Vector2((target.position.x - transform.position.x), 0).normalized * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);

    }
    // Follow Target with AStar
    void FollowWithAstar()
    {
        aStar = new AStarPathfinding(walkable.walkAbleArea);
        
        aStarPath = aStar.FindPath(new Vector2Int((int)transform.position.x + ASTAR_PATH_OFFSET, (int)transform.position.y + ASTAR_PATH_OFFSET),
                new Vector2Int((int)target.position.x + ASTAR_PATH_OFFSET, (int)target.position.y + ASTAR_PATH_OFFSET));
        
        if (aStarPath.Count > 0)
        {
            Vector2 newPath = new Vector2(aStarPath[1].position.x - aStarPath[0].position.x, 0).normalized;
            rb.velocity = newPath * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
            //DrawLine(aStarPath);

            if (aStarPath[1].position.y > aStarPath[0].position.y)
            { 
                if(rb.velocity.x != 0)
                    Jump2(new Vector2(transform.right.x * 40f, jumpForce) * Time.fixedDeltaTime);
            }
        }
        else
        {
            FollowPlayer();
        }
    }
    //Movement while in Attack Mode
    void AtackMove()
    {
        float distanceToPlayerX = Vector2.SqrMagnitude(new Vector2(transform.position.x - target.position.x, 0));
        float distanceToPlayerY = Vector2.SqrMagnitude(new Vector2(transform.position.y - target.position.y, 0));

        if (distanceToPlayerX < ATTACKING_DISTANCE && distanceToPlayerY < ATTACKING_DISTANCE)
        {
            LookingAtTarget();
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (attackCooldownTimer <= 0)
            {
                canAttack = true;
                attackCooldownTimer = attackCooldown;
                AnimationCaller();
            }
            else
            {
                if (!canAttack)
                {
                    attackMoveLR = true;
                }
            }
        }

        if (distanceToPlayerX < STOPPING_DISTANCE && distanceToPlayerY < STOPPING_DISTANCE)
        {
            LookingAtTarget();
            if (attackMoveLR)
            {
                MoveLeftRight(2f);
            }
        }
        else
        {
            TargetFollowFunctionFull();
        }
    }
    // Simple Moving left Right Function
    void MoveLeftRight()
    {
        int rnd = Random.Range(0, 2);
        if (attackMoveTimer <= 0)
        {
            if (rnd == 0)
                moveRight = true;
            else
                moveRight = false;

            attackMoveTimer = ATTACK_MOVE_TIME;
        }
        if (moveRight)
        {
            RaycastHit2D hitR = Physics2D.Raycast(transform.position, new Vector2(transform.up.x + 1.2f, -transform.up.y).normalized, 1f);
            if (hitR.collider)
            {
                if (hitR.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    moveRight = true;
            }
            else
                moveRight = false;
        }
        else
        {
            RaycastHit2D hitL = Physics2D.Raycast(transform.position, new Vector2(transform.up.x - 1.2f, -transform.up.y).normalized, 1f);
            if (hitL.collider)
            {
                if (hitL.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    moveRight = false;
            }
            else
                moveRight = true;
        }
        if (moveRight)
            rb.velocity = new Vector2(1 * (speed) * Time.fixedDeltaTime, rb.velocity.y); // Move Right
        else
            rb.velocity = new Vector2(-1 * (speed) * Time.fixedDeltaTime, rb.velocity.y); // Move Left
    }
    void MoveLeftRight(float speedDivision)
    {
        LookingAtTarget();
        int rnd = Random.Range(0, 2);
        if (attackMoveTimer <= 0)
        {
            if (rnd == 0)
                moveRight = true;
            else
                moveRight = false;

            attackMoveTimer = ATTACK_MOVE_TIME;
        }
        if (moveRight)
        {
            RaycastHit2D hitR = Physics2D.Raycast(transform.position, new Vector2(transform.up.x + 1.2f, -transform.up.y).normalized, 1f);
            if (hitR.collider)
            {
                if (hitR.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    moveRight = true;
            }
            else
                moveRight = false;
        }
        else
        {
            RaycastHit2D hitL = Physics2D.Raycast(transform.position, new Vector2(transform.up.x - 1.2f, -transform.up.y).normalized, 1f);
            if (hitL.collider)
            {
                if (hitL.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    moveRight = false;
            }
            else
                moveRight = true;
        }
        if (moveRight)
            rb.velocity = new Vector2(1 * (speed / speedDivision) * Time.fixedDeltaTime, rb.velocity.y); // Move Right
        else
            rb.velocity = new Vector2(-1 * (speed / speedDivision) * Time.fixedDeltaTime, rb.velocity.y); // Move Left
    }
   
    /// Atttacking player and Walking Animations
    void AnimationCaller()
    {
        anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));

        if (canAttack)
        {
            anim.SetBool("isAttacking", true);
            DamageTarget(0.4f, damage);
        }
        else
            anim.SetBool("isAttacking", false);

        //if (isHurt)
        //    anim.SetBool("isHurt", true);
        //else
        //    anim.SetBool("isHurt", false);
    }
    // All the Timers
    void Timers()
    {
        moveTimeCounter -= Time.deltaTime;
        jumpTime -= Time.deltaTime;
        attackMoveTimer -= Time.deltaTime;

        if (targetFound)
            attackCooldownTimer -= Time.deltaTime;

        if (jumpTime < 0)
        {
            isJumping = false;
        }
        if (attackCooldownTimer <= 0)
        {
            attackMoveLR = false;
        }
    }
    /// Drawing gizmos and Rays and Stuff
    void DrawRaysInScene()
    {
        if (targetFound)
        {
            Debug.DrawRay(new Vector2(feet.transform.position.x - .5f, transform.position.y), transform.up * 2.5f, Color.red);
            Debug.DrawRay(transform.position, new Vector2(transform.right.x, transform.right.y + 1f) * 2f, Color.red);
        }
        else
            Debug.DrawRay(transform.position, transform.right * 8, Color.red);
    }
    /// Drawing Line that shows walking path
    void DrawLine(List<Node> path)
    {
        //line.positionCount = path.Count;

        //Vector3[] linePoints = new Vector3[path.Count];
        //for (int i = 0; i < path.Count; i++)
        //{
        //    linePoints[i] = new Vector3(path[i].position.x, path[i].position.y, 0);
        //}
        //line.SetPositions(linePoints);
    }
}
