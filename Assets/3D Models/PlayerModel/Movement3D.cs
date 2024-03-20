using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float minMoveSpeed;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 3.0f;
    private Vector3 moveDirection;
    private CharacterController characterController;

    public float MoveSpeed {
        set => moveSpeed = Mathf.Clamp(value, MinMoveSpeed, MaxMoveSpeed);
    }
    public float MinMoveSpeed { get => minMoveSpeed; set => minMoveSpeed = value; }
    public float MaxMoveSpeed { get => maxMoveSpeed; set => maxMoveSpeed = value; }

    void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 중력 적용
        if (characterController.isGrounded == false) {
            moveDirection.y += gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction) {
        moveDirection = new Vector3(direction.x, moveDirection.y, direction.z);
    }

    public void JumpTo() {
        if (characterController.isGrounded == true) {
            moveDirection.y = jumpForce;
        }
    }
}
