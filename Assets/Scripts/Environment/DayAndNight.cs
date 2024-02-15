using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour {
    [SerializeField] private float secondPerRealTimeSecond; // 게임 세계 100초 = 현실 세계 1초

    [SerializeField] private float fogDensityCalc; // 증감량 비율
    [SerializeField] private float nightFogDensity; // 밤 상태의 Fog 밀도
    private float dayFogDensity; // 낮 상태의 Fog 밀도
    private float currentFogDensity;

    [SerializeField] private UnityEngine.Color nightColor;

    private float morningTime = 0f;
    private float sunsetTime = 90f;
    private float nightTime = 180f;

    public Material dayToSunset; // 아침 -> 오후
    public Material sunsetToNight; // 오후 -> 저녁
    public Material nightToDay; // 저녁 -> 아침
    public float blendSpeed = 0.1f; // 블렌딩 속도

    private bool isBlended = false; // 블렌딩 중인지 여부
    private float blendFactor = 0f; // 블렌딩 정도


    void Start() {
        dayFogDensity = RenderSettings.fogDensity;
        blendFactor = 0f;
    }

    void Update() {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.fixedDeltaTime);

        if (transform.eulerAngles.x == nightTime || transform.eulerAngles.x == sunsetTime || transform.eulerAngles.x == morningTime) {
            isBlended = false;
            blendFactor = 0f;
            RenderSettings.skybox.SetFloat("_Blend", 0f);
        }
        if (transform.eulerAngles.x <= 90 && transform.eulerAngles.x >= 0) {
            GameManager.instance.isNight = false;
            GameManager.instance.isSunset = false;
            GameManager.instance.isMorning = true;
            if (!isBlended) {
                StartBlend(nightToDay, dayToSunset);
            }
        }
        else if (transform.eulerAngles.x >= 180f || transform.eulerAngles.x < 0) {
            GameManager.instance.isMorning = false;
            GameManager.instance.isSunset = false;
            GameManager.instance.isNight = true;
            if (!isBlended) {
                StartBlend(sunsetToNight, nightToDay);
            }
        }
        else {
            GameManager.instance.isNight = false;
            GameManager.instance.isMorning = false;
            GameManager.instance.isSunset = true;
            if (!isBlended) {
                StartBlend(dayToSunset, sunsetToNight);
            }
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

    public void StartBlend(Material _material, Material _changedMaterial) {
        // 블렌딩 중이면 blendFactor를 조절하여 블렌딩합니다.
        RenderSettings.skybox = _material;
        blendFactor += Time.deltaTime * blendSpeed;
        Debug.Log(blendFactor + isBlended.ToString());

        blendFactor = Mathf.Clamp01(blendFactor);
        _material.SetFloat("_Blend", blendFactor);

        // 블렌딩이 완료되면 블렌딩 중이 아니라고 표시합니다.
        if (blendFactor >= 1f) {
            RenderSettings.skybox = _changedMaterial;
            RenderSettings.skybox.SetFloat("_Blend", 0f);
            isBlended = true;
            blendFactor = 0f;
        }

    }
}
