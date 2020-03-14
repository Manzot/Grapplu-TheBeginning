using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEntry : MonoBehaviour
{
    public void Awake()
    {
        GameFlow.Instance.Initialize();
    }
    void Start()
    {
        GameFlow.Instance.PostInitialize();
    }


    // Update is called once per frame
    void Update()
    {
        GameFlow.Instance.Refresh();
    }

    private void FixedUpdate()
    {
        GameFlow.Instance.PhysicsRefresh();
    }
}
