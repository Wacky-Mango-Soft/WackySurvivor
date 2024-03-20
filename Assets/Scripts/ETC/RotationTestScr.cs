using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTestScr : MonoBehaviour
{

    [SerializeField] private Transform target;
 
    // Update is called once per frame
    void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Euler(targetRotation.eulerAngles);
    }
}
