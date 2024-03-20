using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private SkinnedMeshRenderer meshRenderer;
    private Color originColor;

    void Awake()
    {
        animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        originColor = meshRenderer.material.color;
    }

    public void TakeDamage(int damage) {
        Debug.Log($"{damage}의 데미지를 입었습니다!");
        animator.SetTrigger("onHit");
        StartCoroutine("OnHitColor");
    }

    private IEnumerator OnHitColor()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = originColor;
    }
}
