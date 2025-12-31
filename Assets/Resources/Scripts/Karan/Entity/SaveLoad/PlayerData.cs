using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] position;
    public int sceneIndex;
    public PlayerData(PlayerController playerController)
    {
        position = new float[2];

        position[0] = playerController.savePoint[0];
        position[1] = playerController.savePoint[1];

        sceneIndex  = playerController.currentSceneIndex;
    }
}

