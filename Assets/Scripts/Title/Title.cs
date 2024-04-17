using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public static Title instance;
#region
    private SaveNLoad theSaveNLoad;

    [SerializeField] GameObject leftMenu;
    [SerializeField] Image loading_UI;

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
    #endregion

    public string sceneName = "GameStage";

    private void Start()
    {
        SoundManager.instance.TitleBgmPlay();
    }

    public void ClickStart()
    {
        Debug.Log("Start");
        SoundManager.instance.TitleBgmStop();
        StartCoroutine(GameStartCoroutine());
    }

    private IEnumerator GameStartCoroutine() {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameStage");
        float progress = operation.progress;
        leftMenu.SetActive(false);
        loading_UI.gameObject.SetActive(true);

        while (!operation.isDone) {
            Debug.Log(operation.progress);
            loading_UI.fillAmount = progress;

            yield return new WaitForSeconds(1f);
        }

        this.gameObject.SetActive(false);
    }

    public void ClickLoad()
    {
        Debug.Log("Load");
        GameManager.instance.isPause = false;
        GameManager.instance.isDied = false;
        StartCoroutine(LoadCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            //Debug.Log(operation.progress);

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
