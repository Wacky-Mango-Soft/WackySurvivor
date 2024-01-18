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
    [SerializeField]
    private QuickSlotController theQuickSlot;

    private Slot[] slots;  // 슬롯들 배열
    private Slot[] quickSlots; // 퀵슬롯의 슬롯들
    private bool isNotPut;
    private int SlotNumber;

    private ItemEffectDatabase theItemEffectDatabase; // #1 Bug fix.

    [SerializeField]
    private ActionController theActionController;
    private bool isInventoryFull = false;  // 인벤토리 퀵슬롯 모두 꽉 찼는지 #1

    // Func for Save
    public Slot[] GetSlots() { return slots; }

    [SerializeField] private Item[] items;

    public void LoadToInven(int _arryNum, string _itemName, int _itemCount)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].name == _itemName)
            {
                slots[_arryNum].AddItem(items[i], _itemCount);
            }
        }
    }

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        quickSlots = go_QuickSlotParent.GetComponentsInChildren<Slot>();
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }

    void Update()
    {
        TryOpenInventory();
        TryCloseInventory();
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

    private void TryCloseInventory() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            invectoryActivated = false;
            CloseInventory();

        }
    }

    private void OpenInventory()
    {
        GameManager.instance.isOpenInventory = true;
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        GameManager.instance.isOpenInventory = false;
        go_InventoryBase.SetActive(false);
        theItemEffectDatabase.HideToolTip(); // #1 인벤토리 상태에서 툴팁을 띄우고 i를 통해 인벤토리 비활성화시 툴팁 사라지지 않는 문제.
    }

    // 퀵슬롯부터 획득 아이템이 채워지도록 구현
    public void AcquireItem(Item _item, int _count = 1)
    {
        theItemEffectDatabase.AppearReset(); // #2 아이템이 채워질때 퀵슬롯이 나오도록 수정

        PutSlot(quickSlots, _item, _count);

        // 퀵슬롯이 위치한 곳에 아이템 획득시 변경
        if (!isNotPut)
            theQuickSlot.IsActivatedQuickSlot(SlotNumber);

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
        if (Item.ItemType.Equipment != _item.itemType && Item.ItemType.Kit != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        SlotNumber = i;
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
                SlotNumber = i;
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

    public int GetItemCount(string _itemName)
    {
        int temp = SearchSlotItem(slots, _itemName);

        return temp != 0 ? temp : SearchSlotItem(quickSlots, _itemName);
    }

    private int SearchSlotItem(Slot[] _slots, string _itemName)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item != null)
            {
                if(_itemName == _slots[i].item.itemName)
                    return _slots[i].itemCount;
            }
        }
        return 0;
    }

    public void SetItemCount(string _itemName, int _itemCount)
    {
        if (!ItemCountAdjust(slots, _itemName, _itemCount))
        {
            // #4 건축으로 퀵슬롯 아이템 소모시 퀵슬롯 활성화
            if (ItemCountAdjust(quickSlots, _itemName, _itemCount))
                theItemEffectDatabase.AppearReset();
        }
    }

    private bool ItemCountAdjust(Slot[] _slots, string _itemName, int _itemCount)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item != null)
            {
                if (_itemName == _slots[i].item.itemName)
                {
                    _slots[i].SetSlotCount(-_itemCount);
                    return true;
                }
            }
        }
        return false;
    }
}