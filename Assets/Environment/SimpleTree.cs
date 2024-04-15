using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTree : MonoBehaviour
{
    [SerializeField] private float destroyTime;  // 나무 제거 시간.
    [SerializeField] private string chop_sound;  // 나무 도끼질시 재생시킬 사운드 이름 
    [SerializeField] private string falldown_sound;  // 나무 쓰러질 때 재생시킬 사운드 이름 
    [SerializeField] private string logChange_sound;  // 나무 쓰러져서 통나무로 바뀔 때 재생시킬 사운드 이름
    [SerializeField] Item acquireItem; // 얻게되는 나무 재료
    [SerializeField] int treeDurability; // 나무의 내구도
    [SerializeField] int acquireItemMaxCount; // 때릴 때마다 얻게되는 나무의 갯수 1 ~ treeAcquire 사이의 랜덤 갯수
    private Inventory theIneventory;

    [SerializeField] private float rotationAmount; // 회전할 각도
    [SerializeField] private float rotationSpeed; // 회전 속도
    [SerializeField] private float returnSpeed; // 되돌아오는 속도
    [SerializeField] private float falldownRotation; // 나무가 쓰러졌을때 z축의 각도
    [SerializeField] private float falldownSpeed; // 나무가 쓰러지는 속도
    private Quaternion startRotation; // 초기 회전 상태
    private Quaternion targetRotation; // 목표 회전 상태
    private bool isRotating = false; // 회전 중인지 여부
    private int durabilityCount = 0;

    private void Start() {
        theIneventory = FindObjectOfType<Inventory>();
        startRotation = transform.rotation; // 초기 회전 상태 저장
    }

    public void Chop(Transform chracter_tf) {

        durabilityCount++;

        int acquireItemCount = UnityEngine.Random.Range(1, acquireItemMaxCount + 1);

        if (durabilityCount == treeDurability) {
            SoundManager.instance.PlaySE(chop_sound);
            SoundManager.instance.PlaySE(falldown_sound);
            StartCoroutine(FallDownCoroutine(chracter_tf));
            theIneventory.AcquireItem(acquireItem, acquireItemCount);
            StartCoroutine(AcquirerItem.instance.AcquireLogCoroutine(acquireItem, acquireItemCount));
            gameObject.tag = "Untagged";
            
            Destroy(gameObject, destroyTime);
        }
        else {
            SoundManager.instance.PlaySE(chop_sound);
            theIneventory.AcquireItem(acquireItem, acquireItemCount);
            StartCoroutine(AcquirerItem.instance.AcquireLogCoroutine(acquireItem, acquireItemCount));
            if (!isRotating) {
                StartCoroutine(RotateObject(chracter_tf));
            }
        }
    }

    IEnumerator FallDownCoroutine(Transform chracter_tf)
    {
        Vector3 direction = (transform.position - chracter_tf.position).normalized;
        targetRotation = Quaternion.Euler(direction.x * falldownRotation, 0f, direction.z * falldownRotation);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * falldownSpeed);
            yield return null;
        }

    }

    IEnumerator RotateObject(Transform chracter_tf)
    {
        Vector3 direction = (transform.position - chracter_tf.position).normalized;
        targetRotation = Quaternion.Euler(direction.x * rotationAmount, 0f, direction.z * rotationAmount);

        isRotating = true;

        // 목표 회전 상태로 회전합니다.
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        
        while (Quaternion.Angle(transform.rotation, startRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * returnSpeed);
            yield return null;
        }

        transform.rotation = startRotation; 
        isRotating = false; 
    }

}
