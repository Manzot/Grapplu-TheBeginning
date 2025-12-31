using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    CinemachineVirtualCamera cinemachineCamera;
    private void Start()
    {
        
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
