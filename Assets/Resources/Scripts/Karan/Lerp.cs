using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    public Transform endTransform;
    public float travelTime;
    float timeElapsed = 0;

    Vector2 startPoint;
    Vector2 endPoint;
    Vector3 target;

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

                transform.position = Vector3.Lerp(startPoint, endPoint, timeElapsed / travelTime);
            }
            else

                transform.position = Vector3.Lerp(endPoint, startPoint, timeElapsed / travelTime);
        }
    }
}

