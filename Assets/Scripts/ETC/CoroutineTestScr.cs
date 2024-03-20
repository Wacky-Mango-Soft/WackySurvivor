using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTestScr : MonoBehaviour
{
    private bool isRotate;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) {
            isRotate = true;
            StartCoroutine(RotateCoroutine());
        }
        if (Input.GetKeyUp(KeyCode.G)) {
            isRotate = false;
            StopAllCoroutines();
        }
    }

    public IEnumerator RotateCoroutine()
    {
        while(isRotate) {
            transform.Rotate(new Vector3(0f, 120f * Time.deltaTime, 0f));
            yield return null;
        }
    }
}
