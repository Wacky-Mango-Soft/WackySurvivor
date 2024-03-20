using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTestScr2 : MonoBehaviour
{
    private float speed = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(180f, 0f, 0f) * Time.deltaTime);
        transform.position += new Vector3(0f, 0f, transform.rotation.x * Time.deltaTime * speed);
    }
}
