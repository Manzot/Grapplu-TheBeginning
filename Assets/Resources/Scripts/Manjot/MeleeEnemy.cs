using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    bool moveRight = true;
    bool targetFound;

    Collider2D groundCheckColi;

    float jumpTIme = 2f;

    public float moveTime;
    float moveTimeCounter;
    public Transform feet;

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void PostInitialize()
    {
        base.PostInitialize();
        //feet = transform.GetChild(0);
    }
    // Update is called once per frame
    public override void Refresh()
    {
        Debug.DrawRay(transform.position, transform.right * 15, Color.red);

        moveTimeCounter -= Time.deltaTime;
        jumpTIme -= Time.deltaTime;

        DirectionFacing();

        if (!targetFound) // Searching for target
        {
            RandomMove();
            TargetFound();
        }
        else // When target is found
        {
            if (Vector2.SqrMagnitude(new Vector2(transform.position.x - target.position.x, 0)) > .5f)
                FollowPlayer();
            else
                rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (target.position.y > transform.position.y + 1.5f) //&& target.gameObject.GetComponent<PlayerController>().Grounded()) // jumping if player is on higer platform
        {
            if (Grounded() && jumpTIme < 0)
                Jump();
        }

    }

    //// Functions////////////////
    ///
    void DirectionFacing()
    {
        if (rb.velocity.x > 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 0));
        else if (rb.velocity.x < 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 180));
    }
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

    bool TargetFound()
    {
        RaycastHit2D hit = Physics2D.Raycast(feet.position, transform.right, 15f);
        if (hit.collider)
        {
            if (hit.collider.gameObject.name == "Player")
            {
                targetFound = true;
                return true;
            }
        }
        return false;
    }

    void FollowPlayer()
    {
        rb.velocity = new Vector2((target.position.x - transform.position.x), 0).normalized * speed * Time.deltaTime + new Vector2(0, rb.velocity.y);
    }

    void Jump()
    {
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Time.deltaTime), ForceMode2D.Impulse);
        jumpTIme = 1f;
    }
    bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground", "IObject"));
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector2(feet.position.x, feet.position.y), .2f);
    }

}
