using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// #1 효과 타입 Enum 레이블로 변환
public enum RestoreType
{
    HP,
    SP,
    DP,
    HUNGRY,
    THIRSTY,
    SATISFY
}

[System.Serializable]
public class ItemEffect
{

    public string itemName;  // 아이템의 이름(Key값으로 사용할 것)
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY, SATISFY 만 가능합니다.")]
    public RestoreType[] part;  // 효과. 어느 부분을 회복하거나 혹은 깎을 포션인지. 포션 하나당 미치는 효과가 여러개일 수 있어 배열.
    public int[] num;  // 수치. 포션 하나당 미치는 효과가 여러개일 수 있어 배열. 그에 따른 수치.
}

public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    [SerializeField]
    private StatusController thePlayerStatus;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private SlotToolTip theSlotToolTip;
    [SerializeField]
    private QuickSlotController theQuickSlotController;
    [SerializeField]
    private Inventory theInventory;

    // Inventory > QuickSlot
    public void AppearReset()
    {
        theQuickSlotController.AppearReset();
    }

    // QuickSlotController > Slot 징검다리
    public void IsActivatedquickSlot(int _num)
    {
        theQuickSlotController.IsActivatedQuickSlot(_num);
    }

    // SlotToolTip > Slot
    public void ShowToolTip(Item _item, Vector3 _pos, bool _isQuickSlot)
    {
        theSlotToolTip.ShowToolTip(_item, _pos, _isQuickSlot);
    }

    // SlotToolTip > Slot
    public void HideToolTip()
    {
        theSlotToolTip.HideToolTip();
    }

    public bool GetIsCoolTime()
    {
        return theQuickSlotController.GetIsCoolTime();
    }

    // Inventory > Slot
    public bool GetIsFull()
    {
        return theInventory.GetIsInventoryFull();
    }

    // Inventory > Slot
    public void SetIsFull(bool _flag)
    {
        theInventory.SetIsInventoryFull(_flag);
    }

    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Equipment)
        {
            // 장착
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(_item.weaponType, _item.itemName));
        }
        if (_item.itemType == Item.ItemType.Used)
        {
            for (int i = 0; i < itemEffects.Length; i++)
            {
                if (itemEffects[i].itemName == _item.itemName)
                {
                    for (int j = 0; j < itemEffects[i].part.Length; j++)
                    {
                        switch (itemEffects[i].part[j])
                        {
                            case RestoreType.HP:
                                thePlayerStatus.IncreaseHP(itemEffects[i].num[j]);
                                break;
                            case RestoreType.SP:
                                thePlayerStatus.IncreaseStamina(itemEffects[i].num[j]);
                                break;
                            case RestoreType.DP:
                                thePlayerStatus.IncreaseDP(itemEffects[i].num[j]);
                                break;
                            case RestoreType.THIRSTY:
                                thePlayerStatus.IncreaseThirsty(itemEffects[i].num[j]);
                                break;
                            case RestoreType.HUNGRY:
                                thePlayerStatus.IncreaseHungry(itemEffects[i].num[j]);
                                break;
                            case RestoreType.SATISFY:
                                break;
                            default:
                                Debug.Log("잘못된 Status 부위. HP, SP, DP, HUNGRY, THIRSTY, SATISFY 만 가능합니다.");
                                break;
                        }
                        Debug.Log(_item.itemName + " 을 사용했습니다.");
                    }
                    return;
                }
            }
            Debug.Log("itemEffectDatabase에 일치하는 itemName이 없습니다.");
        }
    }

}