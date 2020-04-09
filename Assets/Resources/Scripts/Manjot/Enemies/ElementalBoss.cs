﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss : BossUnit
{
    const float PUNCH_ATK_TIME = 1.2f;
    const float BIGLASER_ATK_TIME = 3f;
    const float SMALLLASERS_ATK_TIME = 3f;
    const float SPIKY_ATK_TIME = 2f;
    const float WAIT_TO_ATTACK_TIME = 1f;
    const int BIG_LASER_DAMAGE = 25;
    const int SMALL_LASER_DAMAGE = 10;

    Vector2 dirTowardsTarget;
    float moveTimeCounter;
    float moveTime = 2;
    float RANDOM_Y = 0;
    float waitTimeForAttack;
    bool moveRight;
    bool isMovingAttack;

    int randomAttack;

    ThrowAttacks newBigLaser;
    ThrowAttacks newSmallLaser;
    GameObject laser;

    public Transform laserPos;

    public override void Initialize()
    {
        base.Initialize();
        waitTimeForAttack = WAIT_TO_ATTACK_TIME;
        laser = Resources.Load<GameObject>("Prefabs/Manjot/Laser");
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        randomAttack = Random.Range(0, 4);
    }

    public override void Refresh()
    {
        base.Refresh();
        dirTowardsTarget = (target.position - transform.position);

        if (!Dead())
        {
            Timers();
            LookingAtTarget();
            
            anim.SetBool("is_attacking", isAttacking);
            anim.SetBool("movingTowardsPlayer", isMovingAttack);

            if(waitTimeForAttack <= 0)
            {
                if(!isAttacking)
                    AttackMove();
            }
        }

    }
    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();

        if (!Dead())
        {

            if (newBigLaser)
            {
                if(newBigLaser.spriteRend.size.x < 15 && !newBigLaser.isTriggered)
                {
                    newBigLaser.spriteRend.size = new Vector2(newBigLaser.spriteRend.size.x + 20 * Time.fixedDeltaTime, newBigLaser.spriteRend.size.y);
                }
            }

            if (isAttacking)
            {
                if (!isMovingAttack)
                {
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    if (randomAttack == 0)
                    {
                        PunchAttack();
                    }
                    else if(randomAttack == 2)
                    {
                        SpikyAttack();
                    }
                }

            }
            if (waitTimeForAttack > 0)
            {
                RandomMove();
            }
        }
    }

    public void MoveToPlayer(float speedMultiplier = 1)
    {
      //  anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
        if (!enraged)
            rb.velocity = new Vector2(dirTowardsTarget.normalized.x, dirTowardsTarget.normalized.y / 50) * speed * speedMultiplier* Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        else
        {
            rb.velocity = new Vector2(dirTowardsTarget.normalized.x, dirTowardsTarget.normalized.y / 50) * speed * 2f * speedMultiplier * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        }
    }

    void AttackMove()
    {
        switch (randomAttack)
        {
            case 0:
                isAttacking = true;
                isMovingAttack = true;
                PunchAttack();
                break;
            case 1:
                BigLaserAttack();
                break;
            case 2:
                isAttacking = true;
                isMovingAttack = true;
                SpikyAttack();
                break;
            case 3:
                if (enraged)
                    SmallLasersAttacks();
                else
                    BigLaserAttack();
                break;
            case 4:
                if (enraged)
                    SmallLasersAttacks();
                else
                    BigLaserAttack();
                break;
            default:
                PunchAttack();
                break;
        }
    }

    public void PunchAttack()
    {
        if (dirTowardsTarget.sqrMagnitude <= 3f && Grounded())
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("punchAttack", true);
            TimerDelg.Instance.Add(() => { anim.SetBool("punchAttack", false); isAttacking = false; isMovingAttack = false; moveTimeCounter = 0; randomAttack = Random.Range(0, 6); waitTimeForAttack = WAIT_TO_ATTACK_TIME; }, PUNCH_ATK_TIME);
        }
        else
            MoveToPlayer();
    }

    public void BigLaserAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("bigLaserAttack");
        TimerDelg.Instance.Add(() => { isAttacking = false; moveTimeCounter = 0; randomAttack = Random.Range(0, 6); waitTimeForAttack = WAIT_TO_ATTACK_TIME; }, BIGLASER_ATK_TIME);

    }

    public void SmallLasersAttacks()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        RandomMove();
        anim.SetBool("smallaserAttack", true);
        TimerDelg.Instance.Add(() => { isAttacking = false; moveTimeCounter = 0; anim.SetBool("smallaserAttack", false); randomAttack = Random.Range(0, 6); waitTimeForAttack = WAIT_TO_ATTACK_TIME; }, SMALLLASERS_ATK_TIME);
    }

    public void SpikyAttack()
    {
        if (Grounded())
        {
            anim.SetTrigger("spikyAttack");
            rb.velocity = Vector2.zero;
            TimerDelg.Instance.Add(() => { isAttacking = false; moveTimeCounter = 0; randomAttack = Random.Range(0, 6); waitTimeForAttack = WAIT_TO_ATTACK_TIME; }, SPIKY_ATK_TIME);
        }
        else
        {
            MoveToPlayer(2);
        }
    }

    public void RandomMove()
    {
        
        if (moveTimeCounter <= 0)
        {
            RANDOM_Y = Random.Range(-1f, 1f);
            int rnd = Random.Range(0, 2);

            if (rnd == 0) moveRight = true;
            else moveRight = false;

            moveTimeCounter = moveTime;
        }
       // else
        {
                if (moveRight) rb.velocity = new Vector2(1, RANDOM_Y) * speed * Time.fixedDeltaTime; // Move Right
                else rb.velocity = new Vector2(-1, RANDOM_Y) * speed * Time.fixedDeltaTime; // Move Left
        }

    }

    public void Timers()
    {
        moveTimeCounter -= Time.deltaTime;
        waitTimeForAttack -= Time.deltaTime;
    }

    void CreateThrowable()
    {
        
        {
            newBigLaser = GameObject.Instantiate(laser, laserPos.position, Quaternion.identity, this.transform).GetComponent<ThrowAttacks>();
            newBigLaser.Initialize();
            newBigLaser.damage = BIG_LASER_DAMAGE;
            newBigLaser.Throw(0);
        }

    }
    void CreateThrowableSmall()
    {
        
        {
            newSmallLaser = GameObject.Instantiate(laser, laserPos.position, Quaternion.identity, this.transform).GetComponent<ThrowAttacks>();
            newSmallLaser.Initialize();
            newSmallLaser.damage = SMALL_LASER_DAMAGE;
            newSmallLaser.spriteRend.size = new Vector2(0.7f, newSmallLaser.spriteRend.size.y * 0.6f);
            newSmallLaser.Throw();
        }

    }

}
