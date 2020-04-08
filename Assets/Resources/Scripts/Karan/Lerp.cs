using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    public Transform endTransform;
    public float travelTime;
    float timeElapsed = 0;
    public bool AttachableToPlayer = true;

    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 target;

    public bool toLerp = true;
    bool isGoing = true;

    void Start()
    {
        startPoint = transform.position;
        endPoint = endTransform.position;
        target = endPoint;
    }

    void Update()
    {
        UpdatePosition();
    }


    void UpdatePosition()
    {
        timeElapsed += Time.deltaTime;
            if (timeElapsed >= travelTime)
            {
                timeElapsed = 0;
                isGoing = !isGoing;
            }
            if (toLerp)
            {
                if (isGoing)
                {

                    transform.position = Vector2.Lerp(startPoint, endPoint, timeElapsed / travelTime);
                }
                else

                    transform.position = Vector2.Lerp(endPoint, startPoint, timeElapsed / travelTime);
            }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (AttachableToPlayer)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Hook")
           || collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (AttachableToPlayer)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Hook")
            || collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                collision.gameObject.transform.SetParent(null);
        }
    }
}

