using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public static Title instance;

    private SaveNLoad theSaveNLoad;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public string sceneName = "GameStage";

    private void Start()
    {
        SoundManager.instance.TitleBgmPlay();
    }

    public void ClickStart()
    {
        Debug.Log("Start");
        SoundManager.instance.TitleBgmStop();
        SceneManager.LoadScene(sceneName);
        this.gameObject.SetActive(false);
    }
    public void ClickLoad()
    {
        Debug.Log("Load");
        StartCoroutine(LoadCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);

            yield return null;
        }

        SoundManager.instance.TitleBgmStop();
        theSaveNLoad = GetComponent<SaveNLoad>();
        theSaveNLoad.LoadData();
        this.gameObject.SetActive(false);
    }

    public void ClickSetting()
    {
        Debug.Log("Setting");
    }

    public void ClickExit()
    {
        Debug.Log("game exit");
        Application.Quit();
    }
}
