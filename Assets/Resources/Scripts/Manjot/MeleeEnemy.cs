using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    bool moveRight = true;
    bool targetFound;
    bool canJump = false;

    Collider2D groundCheckColi;

    float jumpTime = 2f;

    public float moveTime;
    float moveTimeCounter;
    public Transform feet;

    Transform target;

    AStarPathfinding aStar;
    SetupWalkableArea walkable;
    List<Node> aStarPath;
    LineRenderer line;
    
    public override void Initialize()
    {
        base.Initialize();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        line = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Line")).GetComponent<LineRenderer>();
    }
    public override void PostInitialize()
    {
        base.PostInitialize();
        walkable = FindObjectOfType<SetupWalkableArea>();
        aStar = new AStarPathfinding(walkable.walkAbleArea);
    }

    public override void Refresh()
    {
        moveTimeCounter -= Time.deltaTime;
        jumpTime -= Time.deltaTime;

        DirectionFacing();
        PlatformCheck();

        if (!targetFound) // Searching for target
        {
            RandomMove();
            TargetFound();
        }
        else // When target is found
        {
            if (target.position.y > transform.position.y + .5f || target.position.y < transform.position.y - .5f)
            {
                if (Grounded())
                {
                    FollowWithAstar();
                }
             
                if (target.position.y > transform.position.y + 1.5f)
                {
                    if (Grounded() && jumpTime < 0 && canJump)
                    Jump();
                }
            }
            else
            {
                if (Vector2.SqrMagnitude(new Vector2(transform.position.x - target.position.x, 0)) > .5f)
                    FollowPlayer();
                else
                    rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (Grounded() && jumpTime < 0)
                Jump();
        }

    }
    //// Functions////////////////
    
    /// To check which direction the enemy is facing
    void DirectionFacing()
    {
        if (rb.velocity.x > 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 0));
        else if (rb.velocity.x < 0)
            transform.rotation = Quaternion.Euler(new Vector2(0, 180));
    }
    /// Random movement of enemies to find player
    void RandomMove()
    {
        if (moveTimeCounter <= 0)
        {
            moveRight = !moveRight;
            moveTimeCounter = moveTime;
        }
        if (moveRight) rb.velocity = new Vector2(1 * speed * Time.deltaTime, rb.velocity.y); // Move Right
        else rb.velocity = new Vector2(-1 * speed * Time.deltaTime, rb.velocity.y); // Move Left
    }
    /// Check to see if the target is in range of enemy or not
    bool TargetFound()
    {
        //Debug.DrawRay(transform.position, transform.right * 15, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(feet.position, transform.right, 15f);
        if (hit.collider)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                targetFound = true;
                return true;
            }
        }
        return false;
    }
    /// Following target 
    void FollowPlayer()
    {
        rb.velocity = new Vector2((target.position.x - transform.position.x), 0).normalized * speed * Time.deltaTime + new Vector2(0, rb.velocity.y);
    }
    void FollowWithAstar()
    {
        Debug.Log("AStar On Work");
        aStar = new AStarPathfinding(walkable.walkAbleArea);
        aStarPath = aStar.FindPath(new Vector2Int((int)transform.position.x, (int)transform.position.y), new Vector2Int((int)target.position.x, (int)target.position.y));
        //DrawLine(aStarPath);
        if (aStarPath.Count > 0)
            rb.velocity = new Vector2(aStarPath[1].position.x - transform.position.x, 0).normalized * speed * Time.deltaTime + new Vector2(0, rb.velocity.y);
        else
            FollowPlayer();
    }
    /// Jumping function
    void Jump()
    {
        rb.AddForce(new Vector2(rb.velocity.x + transform.right.x, jumpForce * Time.deltaTime), ForceMode2D.Impulse);
        jumpTime = 1f;
    }
    /// To check if the enemy is on the ground or not
    bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(new Vector2(feet.position.x, feet.position.y), .2f, LayerMask.GetMask("Ground", "IObject"));
    }
    /// Check if there is any platform to jump
    void PlatformCheck()
    {
        if (targetFound)
        {
            Debug.DrawRay(transform.position, transform.up * 1f, Color.red);
            Debug.DrawRay(transform.position, transform.right * 2f, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1f, LayerMask.GetMask("Ground"));
            RaycastHit2D hit2 = Physics2D.Raycast(feet.position, transform.right, 2f, LayerMask.GetMask("Ground"));
            if (hit.collider || hit2.collider)
                canJump = true;
            else
                canJump = false;
        }
    }
    /// Drawing Line that shows walking path
    void DrawLine(List<Node> path)
    {
        line.positionCount = path.Count;

        Vector3[] linePoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            linePoints[i] = new Vector3(path[i].position.x, path[i].position.y, 0);
        }
        line.SetPositions(linePoints);
    }
    /// Drawing gizmos related to the overlapcircles
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector2(feet.position.x, feet.position.y), .2f);
    }
   
}
