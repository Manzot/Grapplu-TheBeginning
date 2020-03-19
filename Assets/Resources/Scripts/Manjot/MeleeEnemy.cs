using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    bool moveRight = true;
    bool targetFound;
    bool isJumping = false;
   

    Collider2D groundCheckColi;

    float jumpTime = 0.5f;

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
        //aStar = new AStarPathfinding(walkable.walkAbleArea);
    }

    public override void Refresh()
    {
        moveTimeCounter -= Time.deltaTime;
        jumpTime -= Time.deltaTime;

        if(jumpTime < 0)
        {
            isJumping = false;
        }

        DirectionFacing();
        PlatformCheck();

        if (!targetFound) // Searching for target
        {
            RandomMove();
            TargetFound();
        }
        else // When target is found
        {
            if (Vector2.SqrMagnitude(new Vector2(transform.position.x - target.position.x, 0)) > .5f)
            {
                if (!isJumping)
                {
                    FollowWithAstar();
                }
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
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
        Debug.DrawRay(transform.position, transform.right * 5, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 5f);
        if (hit.collider)
        {
            Debug.Log(hit.collider.name);
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
        aStarPath = aStar.FindPath(new Vector2Int((int)transform.position.x + 1, (int)transform.position.y + 1), new Vector2Int((int)target.position.x + 1, (int)target.position.y + 1));
        DrawLine(aStarPath);
        if (aStarPath.Count > 0)
        {
            Vector2 newPath = new Vector2(aStarPath[1].position.x - transform.position.x, 0).normalized;
            rb.velocity = newPath * speed * Time.deltaTime + new Vector2(0, rb.velocity.y);
            if (aStarPath[1].position.y > transform.position.y + 1f)
            {
                if (Grounded() && jumpTime < 0)
                {
                    Jump();
                    isJumping = true;
                }
            }
        }
        else
            FollowPlayer();

       

    }
    /// Jumping function
    void Jump()
    {
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Time.deltaTime), ForceMode2D.Impulse);
        jumpTime = 0.5f;
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
            Debug.DrawRay(transform.position, transform.up * 1.2f, Color.red);
            Debug.DrawRay(transform.position, transform.right * 1.2f, Color.red);
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1.2f, LayerMask.GetMask("Ground"));
            //RaycastHit2D hit2 = Physics2D.Raycast(transform.position, transform.right, 1.2f, LayerMask.GetMask("Ground"));
            //if (hit.collider || hit2.collider)
            //    canJump = true;
            //else
            //    canJump = false;
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
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(new Vector2(feet.position.x, feet.position.y), .2f);
    //}
   
}
