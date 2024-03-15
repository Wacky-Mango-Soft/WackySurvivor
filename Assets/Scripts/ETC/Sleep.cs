using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] GameObject sleep_UI;
    [SerializeField] Slider slider;
    [SerializeField] Text sliderText;


    private void Start() {
        statusController = FindObjectOfType<StatusController>();
        ani = fade_UI.GetComponent<Animator>();
        slider.onValueChanged.AddListener((v) => {
            sliderText.text = v.ToString("0");
        });
    }

    public void TrySleep() {
        if (!GameManager.instance.isSleeping && statusController.CurrentSatisfy < 50f) {
            sleep_UI.SetActive(true);
            GameManager.instance.isOpenSleepSlider = true;
        }
        else {
            StartCoroutine(theActionController.WarningTextCoroutine("아직 피곤하지 않습니다."));
        }
    }

    public void DoSleep() {
        Debug.Log((int)(slider.value));
        int v = (int)(slider.value);
        StartCoroutine(SleepingCoroutine(v));
        
    }

    public void SleepCancle() {
        sleep_UI.SetActive(false);
        GameManager.instance.isOpenSleepSlider = false;
    }

    private IEnumerator SleepingCoroutine(int _int) {
        //Debug.Log("코루틴 실행");
        GameManager.instance.isSleeping = true;
        sleep_UI.SetActive(false);
        fade_UI.SetActive(true);
        ani.SetTrigger("FadeOut");
        statusController.DecreaseHungry(50);
        statusController.DecreaseThirsty(50);
        statusController.IncreseMaxSatisfy(); //시간별 회복시간 구현
        yield return new WaitForSeconds(1f);
        theSun.transform.rotation = Quaternion.Euler(theSun.transform.localEulerAngles.x + (_int * 15f) , 0f, 0f);
        thePlayer.transform.position = sleepPos.position;
        thePlayer.transform.rotation = sleepPos.transform.rotation;
        TimeManager.instance.Time = TimeManager.instance.Time + (_int * 3600f);
        ani.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
        SleepCancle();
        fade_UI.SetActive(false);
        GameManager.instance.isSleeping = false;
        theSaveNLoad.SaveData();
    }
}
