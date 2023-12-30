using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool invectoryActivated = false;  // 인벤토리 활성화 여부. true가 되면 카메라 움직임과 다른 입력을 막을 것이다.

    [SerializeField]
    private GameObject go_InventoryBase; // Inventory_Base 이미지
    [SerializeField]
    private GameObject go_SlotsParent;  // Slot들의 부모인 Grid Setting 
    [SerializeField]
    private GameObject go_QuickSlotParent;  // 퀵슬롯 영역

    private Slot[] slots;  // 슬롯들 배열
    private Slot[] quickSlots; // 퀵슬롯의 슬롯들
    private bool isNotPut;

    private ItemEffectDatabase theItemEffectDatabase; // #1 Bug fix.

    [SerializeField]
    private ActionController theActionController;
    private bool isInventoryFull = false;  // 인벤토리 퀵슬롯 모두 꽉 찼는지 #1

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        quickSlots = go_QuickSlotParent.GetComponentsInChildren<Slot>();
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }

    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            invectoryActivated = !invectoryActivated;

            if (invectoryActivated)
                OpenInventory();
            else
                CloseInventory();

        }
    }

    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
        theItemEffectDatabase.HideToolTip(); // #1 인벤토리 상태에서 툴팁을 띄우고 i를 통해 인벤토리 비활성화시 툴팁 사라지지 않는 문제.
    }

    // 퀵슬롯부터 획득 아이템이 채워지도록 구현
    public void AcquireItem(Item _item, int _count = 1)
    {
        theItemEffectDatabase.AppearReset(); // #2 아이템이 채워질때 퀵슬롯이 나오도록 수정

        PutSlot(quickSlots, _item, _count);
        if (isNotPut)
            PutSlot(slots, _item, _count);

        if (isNotPut)
        {
            isInventoryFull = true;
            StartCoroutine(theActionController.WhenInventoryIsFull());
        }
    }

    private void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        if (Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        _slots[i].SetSlotCount(_count);
                        isNotPut = false;
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item == null)
            {
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                return;
            }
        }
        isNotPut = true;
    }

    public bool GetIsInventoryFull()
    {
        return isInventoryFull;
    }

    public void SetIsInventoryFull(bool _flag)
    {
        isInventoryFull = _flag;
    }

}