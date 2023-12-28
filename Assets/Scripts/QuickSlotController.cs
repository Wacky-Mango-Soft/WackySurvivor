using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private Slot[] quickSlots;  // 퀵슬롯들 (8개)
    [SerializeField] private Transform tf_parent;  // 퀵슬롯들의 부모 오브젝트

    private int selectedSlot;  // 선택된 퀵슬롯의 인덱스 (0~7)
    [SerializeField] private GameObject go_SelectedImage;  // 선택된 퀵슬롯 이미지

    [SerializeField]
    private WeaponManager theWeaponManager;

    [SerializeField] private Transform tf_ItemPos;  // 손 끝 오브젝트. 손 끝에 아이템이 위치도록 Transform 정보 받아올 것
    public static GameObject go_HandItem;   // 손에 든 아이템. static인 이유는 이거 하나 받아오려고 📜QuickSlotController 로딩하는건 낭비라서

    [SerializeField]
    private ItemEffectDatabase theItemEffectDatabase;

    void Start()
    {
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        selectedSlot = 0;
    }

    void Update()
    {
        TryInputNumber();
    }

    private void TryInputNumber()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            ChangeSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            ChangeSlot(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            ChangeSlot(7);
    }

    public void IsActivatedQuickSlot(int _num)
    {
        if (selectedSlot == _num)
        {
            Execute();
            return;
        }
        if (DragSlot.instance != null)
        {
            if (DragSlot.instance.dragSlot != null)
            {
                if (DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot)
                {
                    Execute();
                    return;
                }
            }
        }
    }

    private void ChangeSlot(int _num)
    {
        SelectedSlot(_num);
        Execute();
    }

    private void SelectedSlot(int _num)
    {
        // 선택된 슬롯
        selectedSlot = _num;

        // 선택된 슬롯으로 이미지 이동
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }

    private void Execute()
    {
        if (quickSlots[selectedSlot].item != null)
        {
            if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment)
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            else if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Used)
                ChangeHand(quickSlots[selectedSlot].item);
            else
                ChangeHand();
        }
        else
        {
            ChangeHand();
        }
    }

    private void ChangeHand(Item _item = null)
    {
        StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));

        if (_item != null)
            StartCoroutine(HandItemCoroutine());
    }

    IEnumerator HandItemCoroutine()
    {
        HandController.isActivate = false;
        yield return new WaitUntil(() => HandController.isActivate);  // 맨손 교체의 마지막 과정

        go_HandItem = Instantiate(quickSlots[selectedSlot].item.itemPrefab, tf_ItemPos.position, tf_ItemPos.rotation);
        go_HandItem.GetComponent<Rigidbody>().isKinematic = true;  // 중력 영향 X 
        go_HandItem.GetComponent<Collider>().enabled = false;  // 콜라이더 끔 (플레이어와 충돌하지 않게)
        go_HandItem.tag = "Untagged";   // 획득 안되도록 레이어 태그 바꿈
        go_HandItem.layer = 8;  // "Weapon" 레이어는 int
        go_HandItem.transform.SetParent(tf_ItemPos);
    }

    public void EatItem()
    {
        theItemEffectDatabase.UseItem(quickSlots[selectedSlot].item);
        quickSlots[selectedSlot].SetSlotCount(-1);

        if (quickSlots[selectedSlot].itemCount <= 0)
            Destroy(go_HandItem);
    }
}