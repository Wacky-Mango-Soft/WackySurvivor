using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPerRealTimeSecond; // 게임 세계 100초 = 현실 세계 1초

    [SerializeField] private float fogDensityCalc; // 증감량 비율
    [SerializeField] private float nightFogDensity; // 밤 상태의 Fog 밀도
    private float dayFogDensity; // 낮 상태의 Fog 밀도
    private float currentFogDensity;

    [SerializeField] private UnityEngine.Color sunsetColor;
    private float sunsetDensity;
    [SerializeField] private UnityEngine.Color nightColor;

    private float nightTime = 170f;
    private float morningTime = -10f;
    private float sunsetTime = 80;

    // Start is called before the first frame update
    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
        sunsetDensity = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        if (transform.eulerAngles.x >= 170) {
            GameManager.instance.isMorning = false;
            GameManager.instance.isNight = true;
        }
        else if (transform.eulerAngles.x >= -10) {
            GameManager.instance.isNight = false;
            GameManager.instance.isMorning = true;
        }

        if (GameManager.instance.isNight) {
            if (currentFogDensity <= nightFogDensity) {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else {
            if (currentFogDensity >= dayFogDensity) {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
