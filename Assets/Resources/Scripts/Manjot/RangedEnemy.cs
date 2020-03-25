﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * this is going to be fking great game
 * maybe :)
 * muhahahahahaha
 */

public class RangedEnemy : EnemyUnit
{
    const float ATTACK_MOVE_TIMER = 2f;
    const float RUNAWAY_DISTANCE = 2f;
    const float STOPPING_DISTANCE = 6f;
    const float SAME_Y_AS_PLAYER = 0.5f;
    const float PLATFROM_COLI_CHECK_DISTANCE = 1.2f;

    Vector2 dirOppToPlayer;
    GameObject fireball;
    Transform throwPoint;

    bool isRuuningFromPlayer;
    
    float attackMoveTimer = 1f;

    public override void Initialize()
    {
        base.Initialize();
        attackCooldown = Random.Range(3f, 5f);
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        fireball = Resources.Load<GameObject>("Prefabs/Manjot/FireBall");
        throwPoint = transform.GetChild(1);
    }

    public override void Refresh()
    {
        //DrawRays();
        if (!Death())
        {
            Timers();
            DirectionFacingWhenMoving();
            FindTarget();
            AnimationCaller();

            if (!targetFound)
            {
                RandomMove();
            }
            else
            {
                AttackMove();
            }
        }
    }

    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();
    }
    void Timers()
    {
        moveTimeCounter -= Time.deltaTime;
        attackMoveTimer -= Time.deltaTime;
        attackCooldownTimer -= Time.deltaTime;
        jumpTime -= Time.deltaTime;

        if (jumpTime < 0)
        {
            isJumping = false;
        }
    }
    void AnimationCaller()
    {
        anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
        if (targetFound)
        {
            if (canAttack && !isRuuningFromPlayer)
            {
                anim.SetBool("isAttacking", true);
            }
            else
            {
                anim.SetBool("isAttacking", false);
                if(!isJumping)
                    AttackFunction();
            }
        }
    }
    void AttackMove()
    {
        if (!isRuuningFromPlayer && !isJumping)
        {
            if (attackCooldownTimer <= 0 && Grounded())
            {
                canAttack = true;
                rb.velocity = Vector2.zero;
                attackCooldownTimer = attackCooldown;
            }
            LookingAtTarget();
        }
    }
    void AttackFunction()
    {
        float distanceToPlayerX = new Vector2(transform.position.x - target.position.x, 0).sqrMagnitude;
        float distanceToPlayerY = new Vector2(0, transform.position.y - target.position.y).sqrMagnitude;

        dirOppToPlayer = (transform.position - target.position).normalized;

        if (distanceToPlayerX < RUNAWAY_DISTANCE && distanceToPlayerY < SAME_Y_AS_PLAYER)
            isRuuningFromPlayer = true;
        else
            isRuuningFromPlayer = false;

        if (!isRuuningFromPlayer)
        {
            if(distanceToPlayerY > 0.5f) // if both at different Y
            {
                MoveLeftRight();
            }
            else // if both at same y
            {
                if (distanceToPlayerX > STOPPING_DISTANCE + 4) {
                    MoveLeftRight();
                } else {
                    rb.velocity = new Vector2(dirOppToPlayer.x, rb.velocity.y) * (speed / 1.5f) * Time.fixedDeltaTime;
                }                   
            }
        }
        else
        {
            RunAwayFromTarget();
        }


    }
    void MoveLeftRight()
    {
        if (attackMoveTimer <= 0)
        {
            moveRight = !moveRight;
            attackMoveTimer = ATTACK_MOVE_TIMER;
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
            rb.velocity = new Vector2(1 * (speed / 1.5f) * Time.fixedDeltaTime, rb.velocity.y); // Move Right
        else
            rb.velocity = new Vector2(-1 * (speed / 1.5f) * Time.fixedDeltaTime, rb.velocity.y); // Move Left
    }
    void RunAwayFromTarget()
    {
        rb.velocity = new Vector2(dirOppToPlayer.x * speed * Time.fixedDeltaTime, rb.velocity.y);

        RaycastHit2D hit = Physics2D.Raycast(Vector2.zero, Vector2.zero);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, transform.up, 1.5f);
        RaycastHit2D hit3 = Physics2D.Raycast(Vector2.zero, Vector2.zero);

        if (rb.velocity.x > 0)
        {
            hit3 = Physics2D.Raycast(transform.position, new Vector2(transform.up.x + 1.2f, -transform.up.y).normalized, 1f);
            hit = Physics2D.Raycast(new Vector2(feet.position.x + PLATFROM_COLI_CHECK_DISTANCE, feet.transform.position.y), Vector2.up, 1.5f);
        }
        else
        {
            hit3 = Physics2D.Raycast(transform.position, new Vector2(transform.up.x - 1.2f, -transform.up.y).normalized, 1f);
            hit = Physics2D.Raycast(new Vector2(feet.position.x - PLATFROM_COLI_CHECK_DISTANCE, feet.transform.position.y), Vector2.up, 1.5f);
        }

        if (hit.collider)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Jump2(new Vector2(dirOppToPlayer.x * 60f, jumpForce) * Time.deltaTime);
            }
        }
        if (hit2.collider)
        {
            if (hit2.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Jump2(new Vector2(dirOppToPlayer.x * 10f, jumpForce) * Time.deltaTime);
            }
        }
        if (hit3.collider == null)
        {
            Jump2(new Vector2(dirOppToPlayer.x * 60f, jumpForce) * Time.deltaTime);
        }

    }
    void CreateThrowable()
    {
        GameObject.Instantiate(fireball, throwPoint.position, Quaternion.identity, throwPoint);
    }
    void BoolDisabler()
    {
        canAttack = false;
    }
    void DrawRays()
    {
        Debug.DrawRay(transform.position, new Vector2(transform.up.x + 1.2f, -transform.up.y).normalized, Color.red);
        Debug.DrawRay(transform.position, new Vector2(transform.up.x - 1.2f, -transform.up.y).normalized, Color.red);

        Debug.DrawRay(transform.position, transform.up * 1.5f, Color.red);
        if(rb.velocity.x > 0)
            Debug.DrawRay(new Vector2(feet.position.x + PLATFROM_COLI_CHECK_DISTANCE, feet.transform.position.y + .0f), Vector2.up * 1.5f, Color.red);
        else
            Debug.DrawRay(new Vector2(feet.position.x - PLATFROM_COLI_CHECK_DISTANCE, feet.transform.position.y + .0f), Vector2.up * 1.5f, Color.red);
    }
    void OnDrawGizmos()
    {
       // Gizmos.color = Color.red;
       // //if (rb.velocity.x > 0)
       //     Gizmos.DrawSphere(new Vector2(feet.position.x + PLATFROM_COLI_CHECK_DISTANCE, transform.position.y), .8f);
       //// else if (rb.velocity.x < 0)
       //     Gizmos.DrawSphere(new Vector2(feet.position.x - PLATFROM_COLI_CHECK_DISTANCE, transform.position.y), .8f);
    }
    
}



