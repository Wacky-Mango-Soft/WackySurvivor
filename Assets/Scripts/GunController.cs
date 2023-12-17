using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Gun currentGun;

    private float currentFireRate;

    private bool isReload = false;

    private AudioSource audioSource;
    private bool isFineSightMode = false;

    private Vector3 originPos;

    Vector3 recoilBack;
    Vector3 retroActionRecoilBack;

    private RaycastHit hitInfo;
    [SerializeField] private Camera theCam;

    [SerializeField] private GameObject hitEffectPrefab;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originPos = Vector3.zero;
        recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);
    }

    void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();
        TryFineSight();
    }

    private void GunFireRateCalc() {
        if (currentFireRate > 0) {
            currentFireRate -= Time.deltaTime;
        }
    }

    private void TryFire() {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload) {
            Fire();
        }
    }

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
    private void Shoot()
    {
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate;
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    private void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentGun.range)) {
            GameObject clone = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }

    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

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

    public void CancelFineSight()
    {
        if (isFineSightMode)
        {
            FineSightExit();
        }
    }

    private void FineSight()
    {
        isFineSightMode = true;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        StopAllCoroutines();
        StartCoroutine(FineSightActiveCoroutine());
    }
    
    private void FineSightExit()
    {
        isFineSightMode = false;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        StopAllCoroutines();
        StartCoroutine(FineSightDeActiveCoroutine());
    }

    IEnumerator FineSightActiveCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }
    
    IEnumerator FineSightDeActiveCoroutine()
    {
        while(currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

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

    private void PlaySE(AudioClip _clip) {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun() {
        return currentGun;
    }

}
