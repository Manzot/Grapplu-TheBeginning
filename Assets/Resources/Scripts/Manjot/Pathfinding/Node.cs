using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public enum State { Untested, Open, Closed}
    public Vector2Int position;
    public bool isWalkable;
    public float G;
    public float H;
    public float F { get { return G + H; } }
    public Node parent;
    public State state;

    public Node(Vector2Int _position, bool _isWalkable)
    {
        position = _position;
        isWalkable = _isWalkable;
        state = State.Untested;
    }
}
