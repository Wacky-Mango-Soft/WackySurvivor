using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestScripts1 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] Vector3 distance = new Vector3(4f, 0f, 0f);
    private float currAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        distance = new Vector3(transform.position.x - target.transform.position.x, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // transform.Rotate(new Vector3(0f, -90f, 0f) * Time.deltaTime, Space.World);
        // transform.RotateAround(target.position, target.up, Time.deltaTime * rotateSpeed);
        // RotateAroundCD(target.up, distance, rotateSpeed, ref currAngle);
        RotateAroundCDF(target.up, distance, rotateSpeed, ref currAngle);
    }

    private void RotateAroundCD(Vector3 axis, Vector3 dist, float speed, ref float curr)
    {
        curr += speed * Time.deltaTime;
        Vector3 offset = Quaternion.AngleAxis(curr, axis) * dist;
        transform.position = target.position + offset;
    }

    private void RotateAroundCDF(Vector3 axis, Vector3 dist, float speed, ref float curr)
    {
        curr += speed * Time.deltaTime;
        Vector3 offset = Quaternion.AngleAxis(curr, axis) * dist;
        transform.position = target.position + offset;
        transform.rotation = Quaternion.LookRotation(-offset, axis);
    }
}
