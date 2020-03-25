using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    const float STOPPING_DISTANCE = 0.3f;
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
       
    }
    /// Update Function
    public override void Refresh()
    {
        if (!Death())
        {
            Timers();
            AnimationsCaller();
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
                        LookingAtTarget();
                        canAttack = true;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
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
                Jump(new Vector2(rb.velocity.x, jumpForce) * Time.fixedDeltaTime);
        }
    }
   
    /// Atttacking player and Walking Animations
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
