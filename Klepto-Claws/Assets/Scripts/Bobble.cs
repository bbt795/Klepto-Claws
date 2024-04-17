using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobble : MonoBehaviour
{
    Vector3 startPos;
    float bobbleSpeed = 2.5f;
    float bobbleHeight = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(BobbleCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BobbleCoroutine()
    {
        while (true)
        {
            float newYPos = startPos.y + Mathf.Sin(Time.time * bobbleSpeed) * bobbleHeight;
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            yield return null;
        }
    }
}
