using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    // 활성화 여부
    public static bool isActivate = true;

    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField]
    private ItemEffectDatabase theItemEffectDatabase;

    private void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
        //thePlayerController = FindObjectOfType<PlayerController>();
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
        if (Input.GetButtonDown("Fire2") && !theQuickSlot.GetIsCoolTime())
        {
            currentCloseWeapon.anim.SetTrigger("Eat");
            //#0 아이템 효과 사용
            theItemEffectDatabase.UseItem(theQuickSlot.GetSelectedSlot().item);
            theQuickSlot.DecreaseSelectedItem();
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