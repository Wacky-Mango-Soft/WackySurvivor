using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;  // 아이템 습득이 가능한 최대 거리

    private bool pickupActivated = false;  // 아이템 습득 가능할시 True 

    private RaycastHit hitInfo;  // 충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 한다.

    [SerializeField]
    private Text actionText;  // 행동을 보여 줄 텍스트
    [SerializeField]
    private Text itemFullText;  // 아이템이 꽉 찼다는 경고 메세지를 보여줄 텍스트 #1

    [SerializeField]
    private Inventory theInventory;

    private RaycastHit hitInfo_SphereRay;
    private float castRadius = 1.0f;
    private Vector3 previousCameraForward;


    private void Start()
    {
    }

    void Update()
    {
        CheckItem();
        TryAction();
        CheckDragableItem();
        TryDrag();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void TryDrag()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (CheckDragableItem())
            {
                DragItem();
            }
        }
        else if (Input.GetKeyUp(KeyCode.F) && CheckDragableItem())
        {
            DrowItem();
        }
    }

    private void DragItem()
    {
        hitInfo_SphereRay.transform.position = transform.position + transform.forward * range;
        previousCameraForward = transform.forward;
    }

    private void DrowItem()
    {
        Vector3 currentCameraForward = transform.forward;
        Vector3 throwDirection = currentCameraForward - previousCameraForward;
        float throwPower = ((throwDirection / Time.deltaTime).magnitude) % 10f;
        Debug.Log(throwPower);
        hitInfo_SphereRay.transform.GetComponent<Rigidbody>().AddForce(throwDirection.normalized * throwPower, ForceMode.Impulse);
    }

    private bool CheckDragableItem()
    {
        if (Physics.SphereCast(transform.position, castRadius, transform.forward, out hitInfo_SphereRay, range, layerMask))
        {
            if (hitInfo_SphereRay.transform.tag == "Item")
            {
                return true;
            } 
        }
        return false;
    }

    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
        }
        else
            ItemInfoDisappear();
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>" + " 들기 " + "<color=yellow>" + "(F)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }

    private void CanPickUp()
    {
        if (pickupActivated && !theInventory.GetIsInventoryFull())
        {
            if (hitInfo.transform != null)
            {
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 했습니다.");  // 인벤토리 넣기
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                ItemInfoDisappear();
            }
        }
    }

    // #1
    public IEnumerator WhenInventoryIsFull()
    {
        itemFullText.gameObject.SetActive(true);
        itemFullText.text = "아이템이 가득 찼습니다.";

        yield return new WaitForSeconds(3.0f);  // 3 초 후 메세지는 사라짐. 메세지는 3 초만 띄움.
        itemFullText.gameObject.SetActive(false);
    }
}