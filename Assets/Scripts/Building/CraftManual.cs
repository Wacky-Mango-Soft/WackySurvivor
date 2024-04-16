using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName; //이름
    public Sprite craftImage; // 이미지
    public string craftDesc; // 설명
    public string[] craftNeedItem; // 필요한 아이템
    public int[] craftNeedItemCount; // 필요한 아이템의 개수
    public GameObject go_Prefab; //실제 설치될 프리팹
    public GameObject go_PreviewPrefab;
}


public class CraftManual : MonoBehaviour
{
    // 상태변수
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField] private GameObject go_BaseUI; // 기본 베이스 UI

    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_SelectedTab;
    [SerializeField, Range(0, 1f)] private float buildAfterAttackDelay;

    [SerializeField] Craft[] craft_fire; // 모닥불용 탭
    [SerializeField] Craft[] craft_build; // 건축용 탭

    private GameObject go_Preview; // 미리보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수

    [SerializeField] private Transform tf_Player;

    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    // 필요한 UI Slot 요소
    [SerializeField] private GameObject[] go_Slots;
    [SerializeField] private Image[] image_Slot;
    [SerializeField] private Text[] text_SlotName;
    [SerializeField] private Text[] text_SlotDesc;
    [SerializeField] private Text[] text_SlotNeedItem;

    // 필요한 컴포넌트
    private Inventory theInventory;

    private void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        tabNumber = 0;
        page = 1;
        TabSlotSetting(craft_fire);
    }

    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        // 다른 페이지에서 동일 탭 클릭시 1페이지로 이동
        page = 1;

        switch (tabNumber)
        {
            case 0:
                TabSlotSetting(craft_fire);
                break;
            case 1:
                TabSlotSetting(craft_build);
                break;
        }
    }

    private void ClearSlot()
    {
        for (int i = 0; i < go_Slots.Length; i++)
        {
            image_Slot[i].sprite = null;
            text_SlotDesc[i].text = "";
            text_SlotName[i].text = "";
            text_SlotNeedItem[i].text = "";
            go_Slots[i].SetActive(false);
        }
    }

    public void RightPageSetting()
    {
        if (page < (craft_SelectedTab.Length / go_Slots.Length) + 1)
            page++;
        else
            page = 1;

        TabSlotSetting(craft_SelectedTab);
    }

    public void LeftPageSetting()
    {
        if (page != 1)
            page--;
        else
            page = (craft_SelectedTab.Length / go_Slots.Length) + 1;

        TabSlotSetting(craft_SelectedTab);
    }

    private void TabSlotSetting(Craft[] _craft_tab)
    {
        ClearSlot();
        craft_SelectedTab = _craft_tab;

        // 페이저 구현부
        // 0, 4, 8... 4의 배수; 해당 page에 몇번 인덱스의 build 아이템을 보여줄지 계산
        // 0 1 2 3 / 4 5 6 7 / 8 9 10 11
        int startSlotNumber = (page - 1) * go_Slots.Length;

        for (int i = startSlotNumber; i < craft_SelectedTab.Length; i++)
        {
            if (i == page * go_Slots.Length)
                break;

            go_Slots[i - startSlotNumber].SetActive(true);

            image_Slot[i - startSlotNumber].sprite = craft_SelectedTab[i].craftImage;
            text_SlotName[i - startSlotNumber].text = craft_SelectedTab[i].craftName;
            text_SlotDesc[i - startSlotNumber].text = craft_SelectedTab[i].craftDesc;

            for (int x = 0; x < craft_SelectedTab[i].craftNeedItem.Length; x++)
            {
                text_SlotNeedItem[i - startSlotNumber].text += craft_SelectedTab[i].craftNeedItem[x];
                text_SlotNeedItem[i - startSlotNumber].text += " x " + craft_SelectedTab[i].craftNeedItemCount[x] + "\n";
            }
        }
    }

    public void SlotClick(int _slotNumber)
    {
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        if (!CheckIngredient())
            return;

        go_Preview = Instantiate(craft_SelectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_SelectedTab[selectedSlotNumber].go_Prefab;
        isPreviewActivated = true;
        GameManager.instance.isOpenCraftManual = false;

        go_BaseUI.SetActive(false);
    }

    private bool CheckIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if (theInventory.GetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i])
            {
                return false;
            }
        }

        return true;
    }

    private void UseIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            theInventory.SetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i], craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            CloseWindow();
        }

        if (isPreviewActivated) {
            PreviewPositionUpdate();
        }

        if (Input.GetButtonDown("Fire1")) {
            Build();
        }

        if (Input.GetButtonDown("Fire2"))
            Cancel();
    }

    private void Build()
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().IsBuildable())
        {
            UseIngredient();
            Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation);
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated=false;
            go_Preview = null;
            go_Prefab = null;
            StartCoroutine(DontShotDeleyTime());
        }
    }

    private IEnumerator DontShotDeleyTime() {
        yield return new WaitForSeconds(buildAfterAttackDelay);
        GameManager.instance.isBuliding = false;
    }


    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;

                // 회전
                if (Input.GetKeyDown(KeyCode.Q))
                    go_Preview.transform.Rotate(0f, -90f, 0f);
                else if (Input.GetKeyDown(KeyCode.E))
                    go_Preview.transform.Rotate(0f, +90f, 0f);

                // Grid 단위 Building
                _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / 0.1f) * 0.1f, Mathf.Round(_location.z));
                go_Preview.transform.position = _location;
                //Debug.Log(hitInfo.transform.name);

            }
        }
    }

    private void Cancel()
    {
        if (isPreviewActivated)
            Destroy(go_Preview);
        
        isActivated = false;
        isPreviewActivated= false;
        go_Preview = null;
        go_Prefab = null;

        GameManager.instance.isBuliding = false;
        GameManager.instance.isOpenCraftManual = false;

        go_BaseUI.SetActive(false);
    }

    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }

    private void OpenWindow()
    {
        GameManager.instance.isBuliding = true;
        GameManager.instance.isOpenCraftManual = true;
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        GameManager.instance.isBuliding = false;
        GameManager.instance.isOpenCraftManual = false;
        isActivated = false;
        go_BaseUI.SetActive(false);
    }
}
