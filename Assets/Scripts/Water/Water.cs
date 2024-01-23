using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    [SerializeField] private float waterDrag; // 물속 중력
    private float originDrag;

    [SerializeField] private Color waterColor; // 물속 색깔
    [SerializeField] private float waterFogDensity; // 물 탁함 정도

    [SerializeField] private Color waterNightColor;
    [SerializeField] private float waterNightFogDensity;

    private Color originColor;
    private float originFogDensity;

    [SerializeField] private Color originNightColor;
    [SerializeField] private float originNightFogDensity;

    [SerializeField] private string sound_WaterOut;
    [SerializeField] private string sound_WaterIn;
    [SerializeField] private string sound_Breathe;

    [SerializeField] private float breatheTime;
    private float currentBreathTime;

    [SerializeField] private float totalOxygen;
    private float currentOxygen;
    private float frameToSecond; // 산소 고갈 후 체력을 일정 시간마다 감소시키기 위한 임시 변수

    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private Text text_totalOxygen;
    [SerializeField] private Text text_currentOxygen;
    [SerializeField] private Image image_gauge;

    private StatusController thePlayerStat;

    // Start is called before the first frame update
    void Start()
    {
        originColor = RenderSettings.fogColor;
        originFogDensity = RenderSettings.fogDensity;

        originDrag = 0;
        thePlayerStat = FindObjectOfType<StatusController>();
        currentOxygen = totalOxygen;
        text_totalOxygen.text = totalOxygen.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isWater)
        {
            currentBreathTime += Time.deltaTime;
            if (currentBreathTime >= breatheTime)
            {
                SoundManager.instance.PlaySE(sound_Breathe);
                currentBreathTime = 0;
            }
        }

        DecreaseOxygen();
    }

    private void DecreaseOxygen()
    {
        if (GameManager.instance.isWater)
        {
            currentOxygen -= Time.deltaTime;
            text_currentOxygen.text = Mathf.RoundToInt(currentOxygen).ToString();
            image_gauge.fillAmount = currentOxygen / totalOxygen;

            if (currentOxygen <= 0)
            {
                text_currentOxygen.text = "0";
                frameToSecond += Time.deltaTime;
                if (frameToSecond >= 1)
                {
                    thePlayerStat.DecreaseHP(1);
                    frameToSecond = 0;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetWater(other);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetOutWater(other);
        }
    }

    private void GetWater(Collider _player)
    {
        go_BaseUI.SetActive(true);
        SoundManager.instance.PlaySE(sound_WaterIn);

        GameManager.instance.isWater = true;
        _player.transform.GetComponent<Rigidbody>().drag = waterDrag;

        if (!GameManager.instance.isNight)
        {
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFogDensity;
        }
        else
        {
            RenderSettings.fogColor = waterNightColor;
            RenderSettings.fogDensity = waterNightFogDensity;
        }

    }

    private void GetOutWater(Collider _player)
    {
        if (GameManager.instance.isWater)
        {
            go_BaseUI.SetActive(false);
            currentOxygen = totalOxygen;
            text_currentOxygen.text = currentOxygen.ToString();
            image_gauge.fillAmount = 1;
            SoundManager.instance.PlaySE(sound_WaterOut);

            GameManager.instance.isWater = false;
            _player.transform.GetComponent<Rigidbody>().drag = originDrag;

            if (!GameManager.instance.isNight)
            {
                RenderSettings.fogColor = originColor;
                RenderSettings.fogDensity = originFogDensity;
            }
            else
            {
                RenderSettings.fogColor = originNightColor;
                RenderSettings.fogDensity = originNightFogDensity;
            }
        }
    }
}
