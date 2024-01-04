using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 활성화 여부
    public static bool isActivate = false;

    // 현재 장착된 총
    [SerializeField] private Gun currentGun;

    // 연사 속도 계산
    private float currentFireRate;

    // 상태 변수
    private bool isReload = false;
    [HideInInspector] public bool isFineSightMode = false;

    // 본래 포지션 값
    private Vector3 originPos;

    // 효과음 재생
    private AudioSource audioSource;

    // 반동 계산용 변수
    Vector3 recoilBack;
    Vector3 retroActionRecoilBack;

    // 레이저 충돌 정보 받아올 변수
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;

    // 필요한 컴포넌트
    [SerializeField] private Camera theCam;
    private Crosshair theCrosshair;

    // 피격 이펙트
    [SerializeField] private GameObject hitEffectPrefab;

    private void Start()
    {
        originPos = Vector3.zero;
        recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);
        audioSource = GetComponent<AudioSource>();
        theCrosshair = FindObjectOfType<Crosshair>();
    }

    void Update()
    {
        if (isActivate)
        {
            GunFireRateCalc();
            if (!Inventory.invectoryActivated)
            {
                TryFire();
                TryReload();
                TryFineSight();
            }
        }
    }

    // 연사속도 재계산
    private void GunFireRateCalc() {
        if (currentFireRate > 0) {
            currentFireRate -= Time.deltaTime;
        }
    }

    // 발사 시도
    private void TryFire() {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload) {
            Fire();
        }
    }

    // 발사 전 계산
    private void Fire() {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0) {
                Shoot();
            } else {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    // 발사 후 계산
    private void Shoot()
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    // 발사 후 처리
    private void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(UnityEngine.Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        UnityEngine.Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        0)
            , out hitInfo, currentGun.range, layerMask)) {
            GameObject clone = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);

            //#0 총으로 Animal 공격 가능하도록 기능 추가
            Debug.Log(hitInfo.transform.name); // 타겟 확인용 디버깅
            if (hitInfo.transform != null)
            {
                Animal animal = hitInfo.transform.GetComponent<Animal>();
                if (animal != null)
                {
                    animal.Damage(currentGun.damage, transform.position);
                    Debug.Log(hitInfo.transform.name + "에 " + currentGun.damage + "만큼의 데미지");
                }
            }
        }
    }

    // 재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    // 재장전 취소
    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

    // 재장전 처리
    IEnumerator ReloadCoroutine() {
        if (currentGun.carryBulletCount > 0) {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount) {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            isReload = false;
        } 
        else
        {
            Debug.Log("carry bullet count = 0");
        }
    }

    // 정조준 시도
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
        if (Input.GetButtonUp("Fire2") && !isReload)
        {
            FineSightExit();
        }
    }

    // 정조준 취소
    public void CancelFineSight()
    {
        if (isFineSightMode)
        {
            FineSightExit();
        }
    }

    // 정조준 시작
    private void FineSight()
    {
        isFineSightMode = true;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);
        StopAllCoroutines();
        StartCoroutine(FineSightActiveCoroutine());
    }

    // 정조준 해제
    private void FineSightExit()
    {
        isFineSightMode = false;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);
        StopAllCoroutines();
        StartCoroutine(FineSightDeActiveCoroutine());
    }

    // 정조준 활성화 코루틴
    IEnumerator FineSightActiveCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    // 정조준 비활성화 코루틴
    IEnumerator FineSightDeActiveCoroutine()
    {
        while(currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    // 반동 코루틴
    IEnumerator RetroActionCoroutine()
    {
        if(!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos;
            // recoil start
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            // gun recovery position
            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        } else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;
            // fineSightMode recoil start
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }
            // fineSightMode gun recovery position
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    // 사운드 재생
    private void PlaySE(AudioClip _clip) {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    // 현재 Gun return
    public Gun GetGun() {
        return currentGun;
    }

    // 현재 정조준 모드인지 여부 Getter
    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    // 무기 변경
    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }
        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }
}
