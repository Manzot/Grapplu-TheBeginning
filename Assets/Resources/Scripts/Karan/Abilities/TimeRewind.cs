using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRewind : Ability
{
    PlayerController player;
    bool isRewinding;
    List<PointInTime> pointsInTime;
    Rigidbody2D rb;

    public TimeRewind(PlayerController _player) : base(_player)
    {
    }

    public void PostInitialize()
    {
        pointsInTime = new List<PointInTime>();
        player = GameObject.FindObjectOfType<PlayerController>();
        rb = player.GetComponent<Rigidbody2D>();
        
    
    }
    public void Refresh() {
        StartRewind();
        StopRewind();
    }

    private void StopRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
    }

    private void StartRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
    }

    public void PhysicsRefresh() {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    private void Record()
    {
        if (pointsInTime.Count > Mathf.Round(5f / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
        else
        {
            pointsInTime.Insert(0, new PointInTime(player.gameObject.transform.position, player.gameObject.transform.rotation));

        }
    }

    private void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            player.gameObject.transform.position = pointInTime.position;
            player.gameObject.transform.rotation = pointInTime.rotation;
            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }
}
