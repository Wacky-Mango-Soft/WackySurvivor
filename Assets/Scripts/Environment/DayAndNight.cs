using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour {
    //[SerializeField] private float secondPerRealTimeSecond; // sun 돌리기용으로만 쓰여서 삭제

    [SerializeField] private float fogDensityCalc; // 증감량 비율
    [SerializeField] private float nightFogDensity; // 밤 상태의 Fog 밀도
    private float dayFogDensity; // 낮 상태의 Fog 밀도
    private float currentFogDensity;

    [SerializeField] private UnityEngine.Color nightColor;

    public Material dayToSunset; // 아침 -> 오후
    public Material sunsetToNight; // 오후 -> 저녁
    public Material nightToDay; // 저녁 -> 아침
    [SerializeField][Range(0f, 0.1f)] public float blendSpeed; // 블렌딩 속도

    private bool isBlendingStepOver = false;
    private float blendFactor = 0f; // 블렌딩 정도

    const int dayH = 1;
    const int sunsetH = 2;
    const int nightH = 3;


    void Start() {
        dayFogDensity = RenderSettings.fogDensity;
        dayToSunset.SetFloat("_Blend", 0);
        sunsetToNight.SetFloat("_Blend", 1);
        nightToDay.SetFloat("_Blend", 0);
    }

    void FixedUpdate() {
        CheckBlend();
        WorkBlend();
        WorkShade();

        //transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.fixedDeltaTime);
    }
        

    void CheckBlend() {
        if (!isBlendingStepOver) { return; }
        if ((TimeManager.instance.Hour == dayH && TimeManager.instance.Minute == 0) ||
            (TimeManager.instance.Hour == sunsetH && TimeManager.instance.Minute == 0) ||
            (TimeManager.instance.Hour == nightH && TimeManager.instance.Minute == 0)) {
            TimeManager.instance.IsBlended = false;
        }
    }

    void WorkBlend() {
        if (TimeManager.instance.IsBlended) { return; }
        switch (TimeManager.instance.Hour) {
            case dayH:
                GameManager.instance.isMorning = true;
                GameManager.instance.isSunset = false;
                GameManager.instance.isNight = false;
                StartCoroutine(BlendingCoroutine(nightToDay));
                break;
            case sunsetH:
                GameManager.instance.isMorning = false;
                GameManager.instance.isSunset = true;
                GameManager.instance.isNight = false;
                StartCoroutine(BlendingCoroutine(dayToSunset));
                break;
            case nightH:
                GameManager.instance.isMorning = false;
                GameManager.instance.isSunset = false;
                GameManager.instance.isNight = true;
                StartCoroutine(BlendingCoroutine(sunsetToNight));
                break;
        }
    }

    void WorkShade() {
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

    public IEnumerator BlendingCoroutine(Material _material) {
        blendFactor = 0f;
        TimeManager.instance.IsBlended = true;
        RenderSettings.skybox.SetFloat("_Blend", 0f);
        isBlendingStepOver = false;
        RenderSettings.skybox = _material;
        while (!isBlendingStepOver) {
            blendFactor += blendSpeed;
            RenderSettings.skybox.SetFloat("_Blend", blendFactor);
            if (blendFactor >= 1f) {
                isBlendingStepOver = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
        //yield return new WaitForSeconds(2f);
    }

}
