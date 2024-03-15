using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    [SerializeField] private int hp;      
    [SerializeField] private int sp;  
    [SerializeField] private int dp;  
    [SerializeField] private int hungry;
    [SerializeField] private int thirsty;  
    [SerializeField] private int satisfy;  

    [SerializeField] private int spIncreaseSpeed;
    [SerializeField] private int spRechargeTime;
    [SerializeField] private int hungryDecreaseTime;
    [SerializeField] private int thirstyDecreaseTime;
    [SerializeField] private int satisfyDecreaseTime;
    
    private bool spUsed;
    private int currentSpRechargeTime;
    private int currentHungryDecreaseTime;
    private int currentThirstyDecreaseTime;
    private int currentSatisfyDecreaseTime;

    // 필요한 이미지
    [SerializeField] private Image[] images_Gauge;

    // 각 상태를 대표하는 인덱스
    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    PlayerController thePlayerController;

    private float decreaseDelay;
    private float currentdecreaseDelay;

    public int CurrentHp { get; set; }
    public int CurrentSp { get; set; }
    public int CurrentDp { get; set; }
    public int CurrentHungry { get; set; }
    public int CurrentThirsty { get; set; }
    public int CurrentSatisfy { get; set; }

    void Start() {
        CurrentHp = hp;
        CurrentDp = dp;
        CurrentSp = sp;
        CurrentHungry = hungry;
        CurrentThirsty = thirsty;
        CurrentSatisfy = satisfy;

        decreaseDelay = 0.5f;

        thePlayerController = FindObjectOfType<PlayerController>();
    }

    void FixedUpdate() {
        Hungry();
        Thirsty();
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
        Satisfy();
    }

    private void GaugeUpdate() {
        images_Gauge[HP].fillAmount = (float)CurrentHp / hp;
        images_Gauge[SP].fillAmount = (float)CurrentSp / sp;
        images_Gauge[DP].fillAmount = (float)CurrentDp / dp;
        images_Gauge[HUNGRY].fillAmount = (float)CurrentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)CurrentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)CurrentSatisfy / satisfy;
    }

    private void Hungry() {
        if (CurrentHungry > 0) {
            if (currentHungryDecreaseTime <= hungryDecreaseTime)
                currentHungryDecreaseTime++;
            else {
                CurrentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
        else {
            Debug.Log("배고픔 수치가 0 이 되었습니다.");
            currentdecreaseDelay += Time.deltaTime;
            if (currentdecreaseDelay >= decreaseDelay) {
                currentdecreaseDelay = 0;
                DecreaseHP(1);
            }
        }
    }

    private void Thirsty() {
        if (CurrentThirsty > 0) {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
                currentThirstyDecreaseTime++;
            else {
                CurrentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
        else {
            Debug.Log("목마름 수치가 0 이 되었습니다.");
            currentdecreaseDelay += Time.deltaTime;
            if (currentdecreaseDelay >= decreaseDelay) {
                currentdecreaseDelay = 0;
                DecreaseHP(1);
            }
        }
    }

    private void Satisfy() {
        if (CurrentSatisfy > 0) {
            if (currentSatisfyDecreaseTime <= satisfyDecreaseTime)
                currentSatisfyDecreaseTime++;
            else {
                CurrentSatisfy--;
                currentSatisfyDecreaseTime = 0;
            }
        }
        else {
            Debug.Log("sataisfy가 0 이 되었습니다.");
        }
    }

    public void IncreaseHP(int _count) {
        if (CurrentHp + _count < hp)
            CurrentHp += _count;
        else
            CurrentHp = hp;
    }

    public void DecreaseHP(int _count) {
        if (CurrentDp > 0) {
            DecreaseDP(_count);
            return;
        }
        CurrentHp -= _count;

        if (CurrentHp <= 0) {
            Debug.Log("캐릭터의 체력이 0이 되었습니다!!");
            thePlayerController.Die();
        }
    }

    public void IncreaseDP(int _count) {
        if (CurrentDp + _count < dp)
            CurrentDp += _count;
        else
            CurrentDp = dp;
    }

    public void DecreaseDP(int _count) {
        CurrentDp -= _count;

        if (CurrentDp <= 0)
            Debug.Log("캐릭터의 방어력이 0이 되었습니다!!");
    }

    public void IncreaseHungry(int _count) {
        if (CurrentHungry + _count < hungry)
            CurrentHungry += _count;
        else
            CurrentHungry = hungry;
    }

    public void DecreaseHungry(int _count) {
        if (CurrentHungry - _count < 0)
            CurrentHungry = 0;
        else
            CurrentHungry -= _count;
    }

    public void IncreaseThirsty(int _count) {
        if (CurrentThirsty + _count < thirsty)
            CurrentThirsty += _count;
        else
            CurrentThirsty = thirsty;
    }

    public void DecreaseThirsty(int _count) {
        if (CurrentThirsty - _count < 0)
            CurrentThirsty = 0;
        else
            CurrentThirsty -= _count;
    }

    public void IncreaseStamina(int _count) {
        if (CurrentSp + _count > sp)
        {
            CurrentSp = sp;
        }
        else
            CurrentSp += _count;
    }

    public void DecreaseStamina(int _count) {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (CurrentSp - _count > 0)
        {
            CurrentSp -= _count;
        }
        else
            CurrentSp = 0;
    }

    private void SPRechargeTime() {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }

    private void SPRecover() {
        if (!spUsed && CurrentSp < sp)
        {
            CurrentSp += spIncreaseSpeed;
        }
    }
    public void IncreaseMaxThirsty() {
        CurrentThirsty = thirsty;
    }

    public void IncreseMaxSatisfy() {
        CurrentSatisfy = satisfy;
    }
}