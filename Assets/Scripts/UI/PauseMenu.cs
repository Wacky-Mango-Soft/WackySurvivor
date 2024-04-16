using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private SaveNLoad theSaveNLoad;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private GameObject dieBaseUI;


    void Start() {

    }

    void Update()
    {
        if (!GameManager.instance.isAnyOpenUI) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (!GameManager.instance.isPause) {
                    CallMenu();
                }
                else {
                    CloseMenu();
                }
            }
        }
    }

    private void CallMenu()
    {
        GameManager.instance.isPause = true;
        go_BaseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        GameManager.instance.isPause = false;
        go_BaseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ClickSave()
    {
        Debug.Log("save button click");
        theSaveNLoad.SaveData();
        CloseMenu();
    }
    public void ClickLoad()
    {
        Debug.Log("load button click");
        theSaveNLoad.LoadData();
        CloseMenu();
    }
    public void ClickExit()
    {
        Debug.Log("exit button click");
        GameManager.instance.ExitGame();
    }

    public void ClickContinue()
    {
        Debug.Log("continue button click");
        theSaveNLoad.LoadData();
        GameManager.instance.isDied = false;
        dieBaseUI.SetActive(false);
        Time.timeScale = 1f;
    }

}
