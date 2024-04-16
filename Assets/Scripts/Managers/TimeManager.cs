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
    float time = 0f;
    float timeScale = 60f;

    public int Hour {
        get {
            return (int)(Time / 3600f);
        }
    }

    public int Minute {
        get {
            return (int)(Time % 3600f / 60f);
        }
    }

    public int Day { get; set;}
    public bool IsBlended { get; set; }
    public float Time { get => time; set => time = value; }

    void FixedUpdate() {
        Time += UnityEngine.Time.fixedDeltaTime * timeScale;
        //time += Time.time;

        if (Time > secondsInDay) {
            Time = 0;
            Day++;
        }
        timeDebugText.text = $"{Day}일 : {Hour}시간 : {Minute}분";

    }
}
