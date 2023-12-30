using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage;  // 아이템의 이미지

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    private InputNumber theInputNumber;
    private ItemEffectDatabase theItemEffectDatabase;

    [SerializeField] private RectTransform baseRect;  // Inventory_Base 의 영역
    [SerializeField] private RectTransform quickSlotBaseRect; // 퀵슬롯의 영역. 퀵슬롯 영역의 슬롯들을 묶어 관리하는 'Content' 오브젝트가 할당 됨.

    [SerializeField] private bool isQuickSlot;  // 해당 슬롯이 퀵슬롯인지 여부 판단
    [SerializeField] private int quickSlotNumber;  // 퀵슬롯 넘버

    void Start()
    {
        theInputNumber = FindObjectOfType<InputNumber>();
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }

    // 아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if(item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    // 해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        // unknown code
        //if (_count < 0)
        //{
        //    if (theItemEffectDatabase.GetIsFull())
        //        theItemEffectDatabase.SetIsFull(false);
        //}

        if (itemCount <= 0)
        {
            ClearSlot();
            if (isQuickSlot)
                if (QuickSlotController.go_HandItem != null)
                    if (QuickSlotController.go_HandItem.GetComponent<ItemPickUp>().item.itemType == Item.ItemType.Used)
                        Destroy(QuickSlotController.go_HandItem);
        }
    }

    // 해당 슬롯 하나 삭제
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);

        if (theItemEffectDatabase.GetIsFull())
            theItemEffectDatabase.SetIsFull(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                if (!isQuickSlot)  // 인벤토리 우클 (인벤토리에서는 쿨타임 적용 x, 적용하려면 쿨타임 이미지 추가하고 퀵슬롯 컨트롤러하고 똑같이 하면된다
                {
                    theItemEffectDatabase.UseItem(item);
                    if (item.itemType == Item.ItemType.Used)
                        SetSlotCount(-1);
                }
                else if (!theItemEffectDatabase.GetIsCoolTime())  // 퀵슬롯 우클 + 쿨타임 중이 아닐 때
                {
                    theItemEffectDatabase.UseItem(item);
                    if (item.itemType == Item.ItemType.Used)
                        SetSlotCount(-1);
                }
            }
        }
    }

    // 마우스 드래그가 시작 됐을 때 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && Inventory.invectoryActivated) // #2 인벤토리가 아닐땐 드레그 불가
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // 마우스 드래그 중일 때 계속 발생하는 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null && Inventory.invectoryActivated) // #2
            DragSlot.instance.transform.position = eventData.position;
    }

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (Inventory.invectoryActivated) // #2
        {
            // fixed. #1 해상도 및 인터페이스 이동에 따라 가변적으로 적용하도록 변경.
            float baseAndQuickDynamicPos = CalculateMovementDirection(baseRect.transform.localPosition, quickSlotBaseRect.transform.localPosition);

            // 인벤토리 영역 || 퀵슬롯 영역
            if (!((DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin
                && DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax
                && DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin
                && DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax)
                ||
                (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin
                && DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax
                && DragSlot.instance.transform.localPosition.y > quickSlotBaseRect.rect.yMin + baseAndQuickDynamicPos
                && DragSlot.instance.transform.localPosition.y < quickSlotBaseRect.rect.yMax + baseAndQuickDynamicPos)))
            {
                if (DragSlot.instance.dragSlot != null)
                {
                    theInputNumber.Call();
                }

            }
            else
            {
                DragSlot.instance.SetColor(0);
                DragSlot.instance.dragSlot = null;
            }
        }
    }

    // #1
    // Extension method (인터페이스가 y축이 아닌 x축으로 이동했을때를 구하려면 내적을 구할때 조건을 다르게 처리해주면된다)
    // 중앙을 앵커로 삼아놓고 이동했으므로 이렇게 계산할 수 있으니 캔버스 앵커를 마음대로 할거면 다른 방법을 시도할것
    public float CalculateMovementDirection(Vector3 startPoint, Vector3 endPoint)
    {
        // 이동한 거리 계산
        Vector3 movement = endPoint - startPoint;

        // 이동한 거리에 대한 방향과 크기 반환
        Vector3 direction = movement.normalized;
        float distance = movement.magnitude;

        // 내적 구해서 y값 이동에 따른 결과 출력 (양수면 같은방향, 음수면 반대방향, 0이면 수직이나 즉각이다)
        if (Vector3.Dot(direction, Vector3.up) > 0)
            return distance;
        else
            return -distance;
    }


    // 해당 슬롯에 무언가가 마우스 드롭 됐을 때 발생하는 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        if (Inventory.invectoryActivated) //#2
        {
            //Debug.Log("OnDrop");
            if (DragSlot.instance.dragSlot != null)
            {
                ChangeSlot();

                if (isQuickSlot)  // 인벤토리->퀵슬롯 or 퀵슬롯->퀵슬롯
                    theItemEffectDatabase.IsActivatedquickSlot(quickSlotNumber);
                else  // 인벤토리->인벤토리. 퀵슬롯->인벤토리
                {
                    if (DragSlot.instance.dragSlot.isQuickSlot)  // 퀵슬롯->인벤토리
                        theItemEffectDatabase.IsActivatedquickSlot(DragSlot.instance.dragSlot.quickSlotNumber);
                }
            }
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        else
            DragSlot.instance.dragSlot.ClearSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            theItemEffectDatabase.ShowToolTip(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }

    public int GetQuickSlotNumber()
    {
        return quickSlotNumber;
    }
}
