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
    private bool dissolveActivated = false; // 고기 해체 가능할 시 True (돼지 시체를 바라 볼 때)
    private bool isDissolving = false;  // 고기 해체 중일 때 True (중복해서 해체하지 않도록)
    private bool fireLookActivated = false; // 불을 근접해서 바라볼 시 true
    private bool lookComputer = false; // 컴퓨터를 바라볼시 true
    private bool lookArchemyTable = false; // 연금 테이블을 바라볼 시 true
    private bool lookActivatedTrap = false; // 가동된 함정을 바라볼 시 true
    private bool lookWater = false; // 물을 바라볼 시 true(later 물 위에 있을시 구현)
    private bool lookBed = false; // 침대를 바라볼 시 true

    private RaycastHit hitInfo;  // 충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 한다.

    // 필요한 컴포넌트
    [SerializeField]
    private Text actionText;  // 행동을 보여 줄 텍스트
    [SerializeField]
    private Text warningText;  // 아이템이 꽉 찼다는 경고 메세지를 보여줄 텍스트 #1
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    WeaponManager theWeaponManager;  // 고기 해체할 때 기존에 들고 있던 무기를 비활성화하기 위해서
    [SerializeField]
    QuickSlotController theQuickSlot; // 고기 구울때 null 체크용
    [SerializeField]
    private Transform tf_MeatDissolveTool;  // 고기 해체 손. 즉 Meat Knife. 고기 해체할 때 활성화 해야 함
    [SerializeField]
    private ComputerKit theComputer;

    // 드레그용 멤버변수
    private RaycastHit hitInfo_SphereRay;
    private float castRadius = 1.0f;
    private Vector3 previousCameraForward;
    private bool isSaveCoroutinePlaying = false;
    [SerializeField, Range(0f, 1f)] private float saveTimeDuration;

    [SerializeField]
    private string sound_meat; // 고기 해체 소리

    public Text WarningText { get => warningText; set => warningText = value; }

    // #0 Bgm 테스트
    //private void Start()
    //{
    //    SoundManager.instance.PlayRandomBGM();
    //}

    void Update()
    {
        CheckAction();
        TryAction();
        CheckDragableItem();
        TryDrag();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckAction();   
            CanPickUp();  
            CanMeat(); 
            CanDropFire();
            CanComputerPowerOn();
            CanArchemyTableOpen();
            CanReInstallTrap();
            CanSleep();
            CanDrinkWater();
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

        if (isSaveCoroutinePlaying)
            return;
        else
            StartCoroutine(PreviousCameraSaveCoroutine());
    }

    private IEnumerator PreviousCameraSaveCoroutine()
    {
        isSaveCoroutinePlaying = true;
        previousCameraForward = transform.forward;
        yield return new WaitForSeconds(saveTimeDuration);
        isSaveCoroutinePlaying = false;
    }

    private void DrowItem()
    {
        Vector3 currentCameraForward = transform.forward;
        Vector3 throwDirection = currentCameraForward - previousCameraForward;
        float throwPower = (throwDirection / Time.deltaTime).magnitude % 10f;
        //Debug.Log(throwPower);
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

    private void CanDropFire()
    {
        if (fireLookActivated)
        {
            if (hitInfo.transform.tag == "Fire" && hitInfo.transform.GetComponent<Fire>().GetIsFire())
            {
                // 손에 든 아이템을 불에 넣음 (선택된 퀵슬롯의 아이템이 null일 가능성)
                Slot _selectedSlot = theQuickSlot.GetSelectedSlot();
                if (_selectedSlot.item != null)
                {
                    DropAnItem(_selectedSlot);
                }
            }
        }
    }

    private void DropAnItem(Slot _selectedSlot)
    {
        switch(_selectedSlot.item.itemType)
        {
            case Item.ItemType.Used:
                if (_selectedSlot.item.itemName.Contains("고기"))
                {
                    Instantiate(_selectedSlot.item.itemPrefab, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    theQuickSlot.DecreaseSelectedItem();
                }
                break;
            case Item.ItemType.Ingredient:
                break;
        }
    }

    private void CheckAction()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            // Debug.Log(hitInfo.transform.name + " <" + hitInfo.transform.tag + "> 레이중...");
            if (hitInfo.transform.tag == "Item")
                ItemInfoAppear();
            else if (hitInfo.transform.tag == "Weak_Animal" || hitInfo.transform.tag == "Strong_Animal")
                MeatInfoAppear();
            else if (hitInfo.transform.tag == "Fire")
                FireInfoAppear();
            else if (hitInfo.transform.tag == "Computer")
                ComputerInfoAppear();
            else if (hitInfo.transform.tag == "ArchemyTable")
                ArchemyInfoAppear();
            else if (hitInfo.transform.tag == "Trap")
                TrapInfoAppear();
            else if (hitInfo.transform.tag == "Water")
                WaterInfoAppear();
            else if (hitInfo.transform.tag == "Bed")
                BedInfoAppear();
            else
                InfoDisappear();
        }
        else
            InfoDisappear();
    }

    // item > fire, fire > item, item > pickup 으로 레이가 바로 옮겨가는 상황 방지용 리셋 함수
    private void Reset()
    {
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
    }


    private void ItemInfoAppear()
    {
        Reset();
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>" + " 들기 " + "<color=yellow>" + "(F)" + "</color>";
    }

    private void MeatInfoAppear()
    {
        //#2 해당 동물로부터 아이템 획득이 가능한 경우에만 조건 추가
        if (hitInfo.transform.GetComponent<Animal>().GetIsDead() 
            && hitInfo.transform.GetComponent<Animal>().GetIsItemExsist())
        {
            Reset();
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<Animal>().GetAnimalName() + " 해체하기 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void FireInfoAppear()
    {
        Reset();
        fireLookActivated = true;

        if (hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "선택된 아이템 불에 넣기 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void ComputerInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn) 
        {
            Reset();
            lookComputer = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "컴퓨터 가동 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void ArchemyInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ArchemyTable>().GetIsOpen())
        {
            Reset();
            lookArchemyTable = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "연금 테이블 조작 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void TrapInfoAppear()
    {
        if (hitInfo.transform.GetComponent<DeadTrap>().GetIsActivated())
        {
            Reset();
            lookActivatedTrap = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "함정 재설치 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    private void BedInfoAppear() {
        Reset();
        lookBed = true;
        //Debug.Log(lookBed);
        actionText.gameObject.SetActive(true);
        actionText.text = "잠 자기 " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void WaterInfoAppear() {
        Reset();
        lookWater = true;
        actionText.gameObject.SetActive(true);
        actionText.text = "물 마시기 " + "<color=yellow>" + "(E)" + "</color>";
    }


    private void CanSleep() {
        if (lookBed) {
            FindObjectOfType<Sleep>().DoSleep();
            InfoDisappear();
        }
    }

    private void CanDrinkWater() {
        if (lookWater) {
            FindObjectOfType<StatusController>().IncreaseMaxThirsty();
            InfoDisappear();
        }
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
        lookComputer = false;
        lookArchemyTable = false;
        lookActivatedTrap = false;
        lookWater = false;
        lookBed = false;
        actionText.gameObject.SetActive(false);
    }

    private void CanPickUp()
    {
        //#3 인벤토리 꽉찰 시 픽업 불가 조건 추가
        if (pickupActivated && !theInventory.GetIsInventoryFull())
        {
            if (hitInfo.transform != null)
            {
                //Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 했습니다.");  // 인벤토리 넣기
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }

    private void CanComputerPowerOn()
    {
        if (lookComputer)
        {
            if (hitInfo.transform != null)
            {
                if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
                {
                    hitInfo.transform.GetComponent<ComputerKit>().PowerOn();
                    InfoDisappear();
                }
            }
        }
    }

    private void CanArchemyTableOpen()
    {
        if (lookArchemyTable)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<ArchemyTable>().Window();
                InfoDisappear();
            }
        }
    }

    private void CanReInstallTrap()
    {
        if (lookActivatedTrap)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<DeadTrap>().ReInstall();
                InfoDisappear();
            }
        }
    }

    private void CanMeat()
    {
        if (dissolveActivated)
        {
            if ((hitInfo.transform.tag == "Weak_Animal" || hitInfo.transform.tag == "Strong_Animal")
                && hitInfo.transform.GetComponent<Animal>().GetIsDead()
                && !isDissolving)
            {
                isDissolving = true;
                InfoDisappear();

                StartCoroutine(MeatCoroutine()); // 고기 해체 실시
            }
        }
    }

    IEnumerator MeatCoroutine()
    {
        WeaponManager.isChangeWeapon = true;  // 고기 해체 중에 무기가 교체되지 않도록
        WeaponSway.isActivated = false;

        // 들고 있던 무기 비활
        WeaponManager.currentWeaponAnim.SetTrigger("Weapon_Out");
        PlayerController.isActivated = false;
        yield return new WaitForSeconds(0.2f);  // 애니메이션 재생 후 비활되도록
        WeaponManager.currentWeapon.gameObject.SetActive(false);

        // 칼 꺼내기
        tf_MeatDissolveTool.gameObject.SetActive(true);  // 애니메이션은 이때 자동으로 실행됨 (디폴트상태니까)
        yield return new WaitForSeconds(0.2f);  // 애니메이션 0.2초 진행 후
        SoundManager.instance.PlaySE(sound_meat);  // 고기 해체 소리
        yield return new WaitForSeconds(1.8f);  // 칼 해체하는 애니메이션 다 끝나길 기다림

        // 고기 아이템 얻기 #4 Animal로 확장
        theInventory.AcquireItem(hitInfo.transform.GetComponent<Animal>().GetItem(), hitInfo.transform.GetComponent<Animal>().itemNumber);

        // 칼 해체 손은 집어 넣고 다시 원래 무기로
        WeaponManager.currentWeapon.gameObject.SetActive(true);
        tf_MeatDissolveTool.gameObject.SetActive(false);

        PlayerController.isActivated = true;
        WeaponSway.isActivated = true;
        WeaponManager.isChangeWeapon = false;  // 다시 무기 교체가 가능하도록
        isDissolving = false;
    }

    // #1
    public IEnumerator WarningTextCoroutine(string _text)
    {
        WarningText.gameObject.SetActive(true);
        WarningText.text = _text;

        yield return new WaitForSeconds(3.0f);  // 3 초 후 메세지는 사라짐. 메세지는 3 초만 띄움.
        WarningText.gameObject.SetActive(false);
    }
}