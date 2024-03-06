using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour {
    [SerializeField] GameObject bed;
    [SerializeField] Transform sleepPos;
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject theSun;
    StatusController statusController;
    [SerializeField] GameObject fade_UI;
    Animator ani;
    [SerializeField] private SaveNLoad theSaveNLoad;
    [SerializeField] ActionController theActionController;

    private void Start() {
        statusController = FindObjectOfType<StatusController>();
        ani = fade_UI.GetComponent<Animator>();
    }

    public void DoSleep() {
        if (!GameManager.instance.isSleeping && GameManager.instance.isNight)
            StartCoroutine(SleepingCoroutine());
        else {
            StartCoroutine(theActionController.WarningTextCoroutine("아직 피곤하지 않습니다."));
        }
    }

    private IEnumerator SleepingCoroutine() {
        //Debug.Log("코루틴 실행");
        GameManager.instance.isSleeping = true;
        fade_UI.SetActive(true);
        ani.SetTrigger("FadeOut");
        statusController.DecreaseHungry(50);
        statusController.DecreaseThirsty(50);
        statusController.IncreseMaxSatisfy();
        yield return new WaitForSeconds(1f);
        //theSun.transform.rotation = Quaternion.Euler(-10, 0, 0);
        thePlayer.transform.position = sleepPos.position;
        thePlayer.transform.rotation = sleepPos.transform.rotation;
        ani.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
        fade_UI.SetActive(false);
        GameManager.instance.isSleeping = false;
        theSaveNLoad.SaveData();
    }
}
