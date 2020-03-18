using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    float startPos;
    float length;
    public float parallaxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (Camera.main.transform.position.x * parallaxSpeed);
        float exceedLength = (Camera.main.transform.position.x * (1 - parallaxSpeed));
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (exceedLength > startPos + length)
            startPos += length;
        else if (exceedLength < startPos - length)
            startPos -= length;
    }
}
