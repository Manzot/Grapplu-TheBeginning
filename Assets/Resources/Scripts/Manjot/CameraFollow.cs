using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Cinemachine.CinemachineVirtualCamera cinemachineCamera;
    private void Start()
    {
        
        cinemachineCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        cinemachineCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
