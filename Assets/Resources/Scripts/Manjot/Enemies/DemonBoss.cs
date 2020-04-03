using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss : BossUnit
{
    const float maxGravity = -12f;
    Vector2 dirTowardsTarget;
    bool isSpawning = true;

    float attackCooldownTimer;
    bool isJumping;

    float kickCooldown = 1.5f;
    float slashCooldown = 2f;
    float jumpCooldown = 9f;

    float kickCooldownTimer;
    float slashCooldownTimer;
    float jumpCooldownTimer;

    public Transform slashPos;

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
        kickCooldownTimer = kickCooldown;
        TimerDelg.Instance.Add(() => { isSpawning = false; }, 3f);
    }

    public override void Refresh()
    {
        base.Refresh();
        if (!Dead())
        {
            anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
            if (!isAttacking)
            {
                anim.SetBool("isAttacking", false);
            }
            Timers();
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
                rb.velocity = new Vector2(dirTowardsTarget.x, 0) * speed * 2f * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);

    }

    void AttackMove()
    {
        if (!Grounded())
        {
            isAttacking = true;
        }
        if (!isAttacking)
        {
            if((target.position - transform.position).sqrMagnitude < 0.8f)
            {
                if(kickCooldownTimer <= 0 && !isAttacking && Grounded())
                    Kick();
            }
            else if((target.position - transform.position).sqrMagnitude > 0.8f && (target.position - transform.position).sqrMagnitude < 3.5f)
            {
                if (slashCooldownTimer <= 0 && !isAttacking && Grounded())
                    SlashAttack1();
                else
                    MoveToPlayer();
            }
            else if((target.position - transform.position).sqrMagnitude > 10f && (target.position - transform.position).sqrMagnitude > 15f)
            {
                if (jumpCooldownTimer <= 0 && !isAttacking && Grounded())
                    JumpAttack();
                else
                    MoveToPlayer();
            }
            else
            {
                if(!isAttacking && Grounded())
                    MoveToPlayer();
            }
        }
        else if (!isAttacking && Grounded())
        {
            MoveToPlayer();
        }
    }

    public void LookingAtTarget()
    {
        if (!isAttacking)
        {
            if (target.position.x < transform.position.x)
                transform.rotation = Quaternion.Euler(new Vector2(0, 180));
            else
                transform.rotation = Quaternion.Euler(new Vector2(0, 0));
        }
    }

    public void SlashAttack1()
    {
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("slash_1");

        Collider2D coli = Physics2D.OverlapCircle(slashPos.position, 0.4f, LayerMask.GetMask("Player"));
        if (coli)
        {
            target.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
      //  slashCooldownTimer = slashCooldown;
    }
    public void SlashAttack2()
    {
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("slash_2");
        
    }
    public void Kick()
    {
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("kick_1");
        Collider2D coli = Physics2D.OverlapCircle(slashPos.position, 0.4f, LayerMask.GetMask("Player"));
        if (coli)
        {
            target.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
        // kickCooldownTimer = kickCooldown;
    }
    public void JumpAttack()
    {
        isAttacking = true;
        slashCooldownTimer = slashCooldown;
        kickCooldownTimer = kickCooldown;
        Vector2 towardsPlayer = (target.position - transform.position).normalized;
        rb.AddForce(new Vector2(towardsPlayer.x * 1000f, jumpForce) * Time.fixedDeltaTime, ForceMode2D.Impulse);
        anim.SetBool("isAttacking", true);
        anim.SetBool("JumpAttack", true);

        Collider2D coli = Physics2D.OverlapCircle(slashPos.position, 0.5f, LayerMask.GetMask("Player"));
        if (coli)
        {
            target.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }

        if (!enraged)
            jumpCooldownTimer = jumpCooldown;
        else
            jumpCooldownTimer = jumpCooldown / 2;
    }
    public void DisableAttack()
    {
        isAttacking = false;
        anim.SetBool("JumpAttack", false);
        if (!enraged)
        {
            slashCooldownTimer = slashCooldown;
            kickCooldownTimer = kickCooldown;
        }
        else
        {
            slashCooldownTimer = slashCooldown / 2;
            kickCooldownTimer = kickCooldown / 2;
        }
    }

    void Timers()
    {
        kickCooldownTimer -= Time.deltaTime;
        slashCooldownTimer -= Time.deltaTime;
        jumpCooldownTimer -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(slashPos.position, 0.4f);
    }
}
