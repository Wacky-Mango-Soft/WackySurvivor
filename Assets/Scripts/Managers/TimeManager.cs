using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {
    public static TimeManager instance;
    #region Singleton
    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    #endregion Singleton

    [SerializeField] Text timeDebugText;

    const float secondsInDay = 86400f;
    float time;
    float timeScale = 60f;

    public int Hour {
        get {
            return (int)(time / 3600f);
        }
    }

    public int Minute {
        get {
            return (int)(time % 3600f / 60f);
        }
    }

    public int Day { get; set;}
    public bool IsBlended { get; set; }

    void FixedUpdate() {
        time += Time.fixedDeltaTime * timeScale;
        //time += Time.time;

        if (time > secondsInDay) {
            time = 0;
            Day++;
        }

        timeDebugText.text = $"{Day}일 : {Hour}시간 : {Minute}분";

        /*if (Hour >= 0 && Hour < 12) {
            GameManager.instance.isMorning = true;
            GameManager.instance.isSunset = false;
            GameManager.instance.isNight = false;
            //skybox blend
        }

        else if (Hour >= 12 && Hour < 18) {
            GameManager.instance.isMorning = false;
            GameManager.instance.isSunset = true;
            GameManager.instance.isNight = false;
        }

        else if (Hour >= 18 && Hour <= 24) {
            GameManager.instance.isMorning = false;
            GameManager.instance.isSunset = false;
            GameManager.instance.isNight = true;
        }*/
    }

    public void TimeStop() {
        Time.timeScale = 0;
        GameManager.instance.isPause = true;

    }

    public void GoTime() {
        Time.timeScale = 1;
        GameManager.instance.isPause = false;
    }
}
