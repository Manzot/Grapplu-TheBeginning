using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{

    Transform cameraPosition;
    Vector3 lastCameraPosition;
    [SerializeField]
    bool infiniteHorizontal = true;
    [SerializeField]
    bool infiniteVertical = false;

    public Vector2 parallaxSpeed;

    float textureSizeX;
    float textureSizeY;
    SpriteRenderer spriteRend;
    Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = Camera.main.transform;
        lastCameraPosition = cameraPosition.position;
        spriteRend = GetComponent<SpriteRenderer>();
        texture = spriteRend.sprite.texture;
        textureSizeX = texture.width / spriteRend.sprite.pixelsPerUnit;
        textureSizeY = texture.height / spriteRend.sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 moveMent = cameraPosition.position - lastCameraPosition;
        transform.position += new Vector3(moveMent.x * parallaxSpeed.x, moveMent.y * parallaxSpeed.y, 0);
        lastCameraPosition = cameraPosition.position;

        if (infiniteHorizontal)
        {
            if(Mathf.Abs(cameraPosition.position.x - transform.position.x) >= textureSizeX)
            {
                float offsetPosX = (cameraPosition.position.x - transform.position.x) % textureSizeX;
                transform.position = new Vector3(cameraPosition.position.x + offsetPosX, transform.position.y);
            }
        }
        if (infiniteVertical)
        {
            if(Mathf.Abs(cameraPosition.position.y - transform.position.y) >= textureSizeY)
            {
                float offsetPosY = (cameraPosition.position.y - transform.position.y) % textureSizeY;
                transform.position = new Vector3(transform.position.x , cameraPosition.position.y + offsetPosY);
            }
        }
    }
}
