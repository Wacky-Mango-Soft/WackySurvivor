using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipState {
        UNARMED,
        TOOLEQUIPMENT,
        TWOHANDED
    }

public class ThirdPlayerController : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField] private KeyCode AttackKeyCode = KeyCode.Mouse0;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private EquipState currentEquipSet;
    private Movement3D movement3D;
    private PlayerAnimator playerAnimator;
    private EquipState equipState;
    private ItemDatabase itemDatabase;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        movement3D = GetComponent<Movement3D>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        itemDatabase = FindObjectOfType<ItemDatabase>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        playerAnimator.OnMovement(x, z);

        movement3D.MoveSpeed = z > 0 ? movement3D.MaxMoveSpeed : movement3D.MinMoveSpeed;

        movement3D.MoveTo(cameraTransform.rotation * new Vector3(x,0,z));

        if (x != 0 || z != 0) {
            transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        }

        if (Input.GetKeyDown(jumpKeyCode)) {
            playerAnimator.OnJump();
            movement3D.JumpTo();
        }

        if (Input.GetKeyDown(AttackKeyCode)) {
            playerAnimator.OnAttack();
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            EquipmentItem equipmentItem = itemDatabase.SearchEquipmentItem("pickaxe");
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            playerAnimator.onPickup();
        }
    }
}
