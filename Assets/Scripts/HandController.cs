using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    // 활성화 여부
    public static bool isActivate = true;

    [SerializeField]
    private QuickSlotController theQuickSlotController;

    private void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
        thePlayerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (isActivate && !Inventory.invectoryActivated)
        {
            if (QuickSlotController.go_HandItem == null)
                TryAttack();
            else
                TryEating();
        }
    }

    private void TryEating()
    {
        if (Input.GetButtonDown("Fire2") && !theQuickSlotController.GetIsCoolTime())
        {
            currentCloseWeapon.anim.SetTrigger("Eat");
            theQuickSlotController.EatItem();
        }
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _CloseWeapon)
    { 
        base.CloseWeaponChange(_CloseWeapon);
        isActivate = true;
    }
}
