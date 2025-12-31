using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime {
    public Vector2 position;
    public Quaternion rotation;

    public PointInTime(Vector2 _Position, Quaternion _Rotation)
    {
        position = _Position;
        rotation=_Rotation;
    }
}
