using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    #region Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton

    public bool canPlayerMove = true; // 플레이어의 움직임 제어.

    public bool isOpenInventory = false; // 인벤토리 활성화
    public bool isOpenCraftManual = false; // 건축 메뉴창 활성화.
    public bool isOpenArchemyTable = false; // 연금 테이블 창 활성화.
    public bool isOpenComputer = false; // 컴퓨터 창 활성화.
    public bool isOpenSleepSlider = false;

    public bool isAnyOpenUI = false;
    
    public bool isBuliding = false;

    public bool isMorning = false;
    public bool isNight = false;
    public bool isSunset = false;

    public bool isSleeping = false;

    public bool isWater = false;

    public bool isPause = false; // 메뉴가 호출되면 true
    public bool isDied = false;
    
    private bool flag = false;
    public bool isSaveDelay = false;
    public bool isOnePersonView = false;

    public bool autoSaveEnable = false;


    private SaveNLoad saveNLoad;
    private WeaponManager theWM;

    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        theWM = FindObjectOfType<WeaponManager>();
        saveNLoad = gameObject.GetComponent<SaveNLoad>();
    }
    // Update is called once per frame
    void Update() {
        if (isOpenInventory || isOpenCraftManual || isOpenArchemyTable || isOpenComputer || isPause || isSleeping || isDied || isOpenSleepSlider) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = true;
        }

        if (isOpenInventory || isOpenCraftManual || isOpenArchemyTable || isOpenComputer || isOpenSleepSlider) {
            isAnyOpenUI = true;
        }
        else {
            isAnyOpenUI = false;
        }

        if (isWater) {
            if (!flag) {
                StopAllCoroutines();
                StartCoroutine(theWM.WeaponInCoroutine());
                flag = true;
            }
        }
        else {
            if (flag) {
                flag = false;
                theWM.WeaponOut();
            }
        }
        AutoSaveCheck();
    }

    private void AutoSaveCheck() {
        if (!autoSaveEnable) { return; }

        if (!isSaveDelay) {
            if (TimeManager.instance.Minute == 30) {
                StartCoroutine(saveNLoad.AutoSaveCoroutine());
            }

        }
    }

    // Use this for initialization
	public void ExitGame() {
        Application.Quit();
    }

    public void ClickExitMainMenu() {
        Debug.Log("Exit Main Menu button click");
        StartCoroutine(ExitMainMenuCoroutine());
    }

    public IEnumerator ExitMainMenuCoroutine() {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameTitle");

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);

            yield return null;
        }

        // isPause = false;
        // isDied = false;
        Time.timeScale = 1f;
        Title.instance.gameObject.SetActive(true);
    }
}
