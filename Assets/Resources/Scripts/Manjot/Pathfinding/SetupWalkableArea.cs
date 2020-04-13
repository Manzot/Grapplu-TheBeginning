using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetupWalkableArea : MonoBehaviour
{
    SpriteRenderer map;
    //public Tilemap map2;
    public Tilemap tilemap;

   public bool[,] walkAbleArea;
    int width;
    int height;
    // Start is called before the first frame update
    void Start()
    {
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
}
