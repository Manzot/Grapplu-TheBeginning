using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    SpriteRenderer map;
    public Tilemap tilemap;

    public Transform start;
    public Transform target;

    LineRenderer line;

    bool[,] walkAbleArea;
    int width;
    int height;
    // Start is called before the first frame update
    void Start()
    {
        line = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Line")).GetComponent<LineRenderer>();
        map = GetComponent<SpriteRenderer>();
        width = (int)map.bounds.size.x;
        height = (int)map.bounds.size.y;
        SetupWalkables();
    }
    
    void SetupWalkables()
    {
        walkAbleArea = new bool[(int)width, (int)height];

        for (int x = (int)map.bounds.min.x; x < width; x++)
        {
            for (int y = (int)map.bounds.min.y; y < height; y++)
            {
                Vector3Int pos = tilemap.WorldToCell(new Vector3Int((int)x, (int)y, 0));
                if (tilemap.GetTile(pos) != null)
                {
                    walkAbleArea[x, y] = true;
                }
                else
                    walkAbleArea[x, y] = false;
            }
        }

    }

    void DrawLine(List<Node> path)
    {
       
        line.positionCount = path.Count;

        Vector3[] linePoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            linePoints[i] = new Vector3(path[i].position.x, path[i].position.y, 0);
        }
        line.SetPositions(linePoints);
    }
}
