using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private SaveNLoad theSaveNLoad;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!GameManager.instance.isPause)
            {
                CallMenu();
            }
            else
            {
                CloseMenu();
            }
        }    
    }

    private void CallMenu()
    {
        GameManager.instance.isPause = true;
        go_BaseUI.SetActive(true);
        Time.timeScale = 0f;

    }

    private void CloseMenu()
    {
        GameManager.instance.isPause = false;
        go_BaseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ClickSave()
    {
        Debug.Log("save button click");
        theSaveNLoad.SaveData();
    }
    public void ClickLoad()
    {
        Debug.Log("load button click");
        theSaveNLoad.LoadData();
    }
    public void ClickExit()
    {
        Debug.Log("exit button click");
        Application.Quit();
    }
}
