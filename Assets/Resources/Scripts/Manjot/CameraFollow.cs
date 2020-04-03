using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private void Start()
    {
        var cinemachineCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        cinemachineCamera.Follow = FindObjectOfType<PlayerController>().transform;
    }
}
