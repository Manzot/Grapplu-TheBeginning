using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum Abilities { Grappler, Rewind, SlowMotion }
public class PlayerController : MonoBehaviour, IDamage
{
    const float SLOMO_FACTOR = 0.3f;

    public Vector2 ropeHook;
    Collider2D groundCheckColi;
    public float speed;
    public float swingForce = 4f;
    public float jumpForce = 3f;
    private float jumpInput;
    public float horizontal;
    public float aimAngle;
    float timeSlowCooldown = 10f;

    bool timeSlow;


    bool isRewinding;
    List<PointInTime> pointsInTime;

    //int jumpCount = 1;

    bool isJumping;
    bool isAttacking;
    public bool groundCheck;
    public bool isSwinging;
    public bool isAlive;

    public Transform feet;
    public Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    public Vector2 angleDirection;
    public Vector3 deathLoc;

    public GameObject crosshair;
    TimeSlowMo timeSlowMo;
    
    //Hook hook;

    public float health = 100f;
    private const float MAX_HEALTH = 100;

    public void Initialize()
    {
        if (health == 0)
        {
            health = MAX_HEALTH;
        }

        isAlive = true;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        timeSlowMo = new TimeSlowMo();
    }
    public void PostInitialize()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody2D>();
       // hook = FindObjectOfType<Hook>();
    }
    public void Refresh()
    {
        MovementAndDoubleJump();
        SetCrosshairPoint(CrossairDirection());

        if (Input.GetKeyDown(KeyCode.T))
        {
            TimeSlowAbility();
        }
        TimeSlowReset();
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            StopRewind();
        }
    }
    public void PhysicsRefresh()
    {

        SwingDirectionForce();
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    private void TimeSlowAbility()
    {
        if (!timeSlow)
        {
            timeSlowMo.SlowMotion(SLOMO_FACTOR);
            timeSlow = true;
        }
    }

    private void TimeSlowReset()
    {
        if (timeSlow)
        {
            if (timeSlowMo.TimeReset(timeSlowCooldown) >= 1)
            {
                timeSlow = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Throwable"))
        {
            if (timeSlow && isAttacking)
            {
                DeflectBullet(collision);
            }
            else
            {
                TakeDamage(5);
                collision.gameObject.SetActive(false);

            }
        }
    }

    private void DeflectBullet(Collider2D collision)
    {
        Rigidbody2D rbGO = collision.gameObject.GetComponent<Rigidbody2D>();
        rbGO.velocity = -1 * rbGO.velocity;
    }

    /*Get a normalized direction vector from the player to the hook point 
     * and  Inverse the direction to get a perpendicular direction based on the HorizontalInput */
    private Vector2 CalculatePerpendicularDirection()
    {
        Vector2 perpendicularDirection;

        var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;
        Debug.DrawLine(transform.position, playerToHookDirection, Color.red, 0f);

        if (horizontal > 0)
        {
            perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
            var leftPerpPos = (Vector2)transform.position + perpendicularDirection * -2f;
            Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
        }
        else
        {

            perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
            var rightPerpPos = (Vector2)transform.position - perpendicularDirection * +2f;
            Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
        }

        return perpendicularDirection;
    }

    /*Movement and jump of the player*/
    public void MovementAndDoubleJump()
    {
        horizontal = Input.GetAxis("Horizontal");
        if (rb.velocity.x > 0)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else if (rb.velocity.x < 0)
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));


        rb.velocity = new Vector2(horizontal * speed * Time.deltaTime, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (Grounded())
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                animator.SetBool("isJumping", true);
                rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Time.deltaTime), ForceMode2D.Impulse);
                isJumping = true;
                TimerDelg.Instance.Add(() => { isJumping = false; }, .5f);
                // jumpCount--;
            }
            else
            {
                animator.SetBool("isJumping", false);
            }
            /* if (jumpCount < 1)
             jumpCount = 1;
            }
            else
            {
                if (jumpCount > 0 && Input.GetKeyDown(KeyCode.Space))
                {
                    rb.AddForce(new Vector2(rb.velocity.x, jumpForce / 1.4f * Time.deltaTime), ForceMode2D.Impulse);
                    jumpCount--;
                }*/
        }
    }

    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(feet.position, 0.1f, LayerMask.GetMask("Ground", "IObject"));
    }

    float CrossairDirection()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        Vector3 faceDirection = worldMousePosition - transform.position;

        aimAngle = Mathf.Atan2(faceDirection.y, faceDirection.x);

        if (aimAngle < 0)
        {
            return aimAngle = Mathf.PI * 2 + aimAngle;
        }
        angleDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        return aimAngle;
    }

    public void SetCrosshairPoint(float aimAngle)
    {

        float x = transform.position.x + 2f * Mathf.Cos(aimAngle);
        float y = transform.position.y + 2f * Mathf.Sin(aimAngle);

        Vector3 crosshairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crosshairPosition;
    }

    public void SwingDirectionForce()
    {
        if (rb.velocity.x < 0f || rb.velocity.x > 0f)
        {

            if (isSwinging)
            {

                Vector2 perpendicularDirection = CalculatePerpendicularDirection();
                /* rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);*/
                var force = perpendicularDirection * swingForce;
                rb.AddForce(force, ForceMode2D.Impulse);
            }

        }
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
        }
    }

    public void DisableBools()
    {
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        animator.SetTrigger("isHurt");
        Debug.Log(health);
        if (health <= 0)
        {
            animator.SetTrigger("isDead");

            isAlive = false;
            deathLoc = this.transform.position;
            this.gameObject.SetActive(false);
            PlayerManager.Instance.IsDead();
        }
    }

    private void StartRewind()
    {
        this.GetComponent<SpriteRenderer>().material.color = Color.blue;
        isRewinding = true;
        rb.isKinematic = true;
    }

    private void StopRewind()
    {
        this.GetComponent<SpriteRenderer>().material.color = Color.white;
        isRewinding = false;
        rb.isKinematic = false;
    }
    private void Record()
    {
        if (pointsInTime.Count > Mathf.Round(2f / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
        else
        {
            pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
        }
    }
    private void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
           
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;

            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }
}



