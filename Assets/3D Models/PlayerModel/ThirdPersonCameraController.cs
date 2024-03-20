using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private float wheelSpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float yMinLimit;
    [SerializeField] private float yMaxLimit;
    [SerializeField] private float xMoveSpeed;
    [SerializeField] private float yMoveSpeed;
    [SerializeField] Transform target;
    [SerializeField] Transform targetOnePersonView;

    private float distance;
    private float x;
    private float y;

    public float X { get => x; set => x = value; }
    public float Y { get => y; set => y = value; }

    // Start is called before the first frame update
    void Awake()
    {
        distance = Vector3.Distance(transform.position, target.position);
        Vector3 angles = transform.eulerAngles;
        X = angles.y;
        Y = angles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null || !GameManager.instance.canPlayerMove) { return; }

        X += Input.GetAxis("Mouse X") * xMoveSpeed * Time.deltaTime;
        Y -= Input.GetAxis("Mouse Y") * yMoveSpeed * Time.deltaTime;
        Y = ClampAngle(Y, yMinLimit, yMaxLimit);

        transform.rotation = Quaternion.Euler(Y,X,0);
        
        distance -= Input.GetAxis("Mouse ScrollWheel") * wheelSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        if (distance == minDistance) {
            GameManager.instance.isOnePersonView = true;
            transform.position = targetOnePersonView.position;
        } else {
            GameManager.instance.isOnePersonView = false;
        }
    }

    void LateUpdate() {
        if (target == null || !GameManager.instance.canPlayerMove) { return; }

        transform.position = transform.rotation * new Vector3(0,0,-distance) + target.position;
    }

    private float ClampAngle(float angle, float min, float max) {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
