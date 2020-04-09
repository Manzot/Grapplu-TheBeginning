using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss : BossUnit
{
    const float maxGravity = -12f;
    const float jumpAttackCD = 2.25f;
    Vector2 dirTowardsTarget;
    bool isSpawning = true;

    float attackCooldownTimer;
    bool isJumping;

    float kickCooldown = 1.5f;
    float slashCooldown = 2f;
    float slash2Cooldown = 2f;
    float jumpCooldown = 9f;

    float kickCooldownTimer;
    float slashCooldownTimer;
    float slash2CooldownTimer;
    float jumpCooldownTimer;

    public override void Initialize()
    {
        base.Initialize();
        rb.velocity = Vector2.zero;
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        jumpCooldownTimer = 10f;
        slashCooldownTimer = slashCooldown;
        slash2CooldownTimer = slash2Cooldown;
        kickCooldownTimer = kickCooldown;
        TimerDelg.Instance.Add(() => { isSpawning = false; }, 3f);
    }

    public override void Refresh()
    {

        base.Refresh();
        if (!Dead())
        {           
            Timers();
            AnimationCaller();
            if(!isAttacking)
                LookingAtTarget();
           
            if (!isSpawning)
                AttackMove();
        }
    }

    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();


    }

    public void MoveToPlayer()
    {
        dirTowardsTarget = (target.position - transform.position).normalized;
        
        if(!enraged)
            rb.velocity = new Vector2(dirTowardsTarget.x, 0) * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        else
        {
            int rnd = Random.Range(0, 16);
            if(rnd == 2 && !isAttacking && Grounded() && slash2CooldownTimer <= 0)
            {
                SlashAttack2();
            }
            rb.velocity = new Vector2(dirTowardsTarget.x, 0) * speed * 2f * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        }
        
    }

    void AnimationCaller()
    {
        anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
        if (!isAttacking)
        {
            anim.SetBool("isAttacking", false);
        }
        else
        {
            anim.SetBool("isAttacking", true);
        }
    }
    void AttackMove()
    {
        if (!isAttacking)
        {
            if((target.position - transform.position).sqrMagnitude < 0.8f)
            {
                if(kickCooldownTimer <= 0 && Grounded())
                {
                    Kick();
                }
            }
            if((target.position - transform.position).sqrMagnitude > 0.8f && (target.position - transform.position).sqrMagnitude < 4.5f)
            {
                if (slashCooldownTimer <= 0 && Grounded())
                {
                    SlashAttack1();
                }
            }
            if((target.position - transform.position).sqrMagnitude > 10f && (target.position - transform.position).sqrMagnitude < 30f)
            {
                if (jumpCooldownTimer <= 0 && Grounded())
                {
                    JumpAttack();
                }
            }
            if(Grounded() && (target.position - transform.position).sqrMagnitude > 0.6f)
                MoveToPlayer();
        }
    }

   

    public void SlashAttack1()
    {
       
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("slash1", true);
        slashCooldownTimer = slashCooldown;
    }
    public void SlashAttack2()
    {
        isAttacking = true;
        rb.velocity = new Vector2(rb.velocity.x/2, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetBool("slash2", true);
        slash2CooldownTimer = slash2Cooldown;
    }
    public void Kick()
    {
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("kick", true);
        kickCooldownTimer = kickCooldown;
    }
    public void JumpAttack()
    {
        isAttacking = true;
        slashCooldownTimer = slashCooldown;
        kickCooldownTimer = kickCooldown;
        anim.SetBool("jump_attack", true);
        Vector2 towardsPlayer = (target.position - transform.position).normalized;
        rb.AddForce(new Vector2(towardsPlayer.x * 1300f, jumpForce) * Time.fixedDeltaTime, ForceMode2D.Impulse);

        if (!enraged)
            jumpCooldownTimer = jumpCooldown;
        else
            jumpCooldownTimer = jumpCooldown / 2;

        TimerDelg.Instance.Add(()=> { isAttacking = false; anim.SetBool("jump_attack", false); }, jumpAttackCD);
    }
    public void DisableAttack()
    {
        isAttacking = false;
        anim.SetBool("slash1", false);
        anim.SetBool("kick", false);
        anim.SetBool("jump_attack", false);
    }


    void Timers()
    {
        kickCooldownTimer -= Time.deltaTime;
        slashCooldownTimer -= Time.deltaTime;
        jumpCooldownTimer -= Time.deltaTime;
        slash2CooldownTimer -= Time.deltaTime;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(slashPos.position, 0.4f);
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            }
        }
    }


}
