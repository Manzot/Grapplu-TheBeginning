using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    const float ATTACKING_DISTANCE = 0.3f;
    const float STOPPING_DISTANCE = 5f;
    const float ATTACK_MOVE_TIME = 1f;

    float attackMoveTimer;
    bool attackMoveLR;
   // LineRenderer line;

    /// Awake Function
    public override void Initialize()
    {
        base.Initialize();
        attackCooldown = Random.Range(2f, 4f);
    }
    //// Start Function
    public override void PostInitialize()
    {
        base.PostInitialize();
       
    }
    /// Update Function
    public override void Refresh()
    {
        if (!Death())
        {

            AnimationCaller();
            Timers();
            Hurt();

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
                    TargetFollowFunctionFull();
                    AtackMove();
                }
            }
        }
    }
    //FollowTarget FInal Complete Version
    void TargetFollowFunctionFull()
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
           
    }
    /// Following target 
    void FollowPlayer()
    {
        if (target.position.y < transform.position.y - .5f)
        {
            rb.velocity = (new Vector2(transform.right.x * speed * Time.deltaTime, rb.velocity.y));
        }
        else
            rb.velocity = new Vector2((target.position.x - transform.position.x), 0).normalized * speed * Time.deltaTime + new Vector2(0, rb.velocity.y);

    }
    // Follow Target with AStar
    void FollowWithAstar()
    {
        aStar = new AStarPathfinding(walkable.walkAbleArea);
        aStarPath = aStar.FindPath(new Vector2Int((int)transform.position.x + ASTAR_PATH_OFFSET, (int)transform.position.y + ASTAR_PATH_OFFSET),
                new Vector2Int((int)target.position.x + ASTAR_PATH_OFFSET, (int)target.position.y + ASTAR_PATH_OFFSET));

        //DrawLine(aStarPath);

        if (aStarPath.Count > 0)
        {
            Vector2 newPath = new Vector2(aStarPath[1].position.x - aStarPath[0].position.x, 0).normalized;
            rb.velocity = newPath * speed * Time.deltaTime + new Vector2(0, rb.velocity.y);

            if (aStarPath[1].position.y > aStarPath[0].position.y)
                Jump(new Vector2(rb.velocity.x, jumpForce) * Time.deltaTime);
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
            if(attackCooldownTimer <= 0)
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

        if(distanceToPlayerX < STOPPING_DISTANCE && distanceToPlayerY < STOPPING_DISTANCE)
        {
            LookingAtTarget();
            if (attackMoveLR)
            {
                MoveLeftRight();
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
            rb.velocity = new Vector2(1 * (speed / 3f) * Time.deltaTime, rb.velocity.y); // Move Right
        else
            rb.velocity = new Vector2(-1 * (speed / 3f) * Time.deltaTime, rb.velocity.y); // Move Left
    }
    /// Atttacking player and Walking Animations
    void AnimationCaller()
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
        attackMoveTimer -= Time.deltaTime;
        attackCooldownTimer -= Time.deltaTime;

        if (jumpTime < 0)
        {
            isJumping = false;
        }
        if(attackCooldownTimer <= 0)
        {
            attackMoveLR = false;
        }
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
    /// Drawing gizmos and Rays and Stuff
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
    void DrawLine(List<Node> path)
    {
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
    }

}
