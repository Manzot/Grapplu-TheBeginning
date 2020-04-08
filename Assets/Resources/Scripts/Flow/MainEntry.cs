using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainEntry : MonoBehaviour
{
    public float customTimeScale;
    public string SceneName;

    public void Awake()
    {
        GameFlow.Instance.Initialize();
    }
    void Start()
    {
        TimeSlowMo timee = new TimeSlowMo();
        timee.InstantResetTime();
        GameFlow.Instance.PostInitialize();
    }


    // Update is called once per frame
    void Update()
    {
        GameFlow.Instance.Refresh();
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(SceneName);
        }
    }

    private void FixedUpdate()
    {
        GameFlow.Instance.PhysicsRefresh();
    }
}
