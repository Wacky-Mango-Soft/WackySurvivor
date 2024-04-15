using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcquirerItem : MonoBehaviour
{
    public static AcquirerItem instance;
    #region Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton

    // 아이템 획득 로그
    [SerializeField] private GameObject acquireBaseUI;
    [SerializeField] private Text acquireText;
    [SerializeField] private Image acquireItemImage;
    [SerializeField] private float acquireLogRemainTime; // 로그 지속시간
    private Coroutine acquireLogCoroutine;

    public IEnumerator AcquireLogCoroutine(Item acquireItem, int acquireItemCount = 1)
    {
        if (acquireLogCoroutine != null)
        {
            StopCoroutine(acquireLogCoroutine);
            acquireLogCoroutine = null;
        }

        acquireBaseUI.SetActive(true);
        acquireItemImage.sprite = acquireItem.itemImage;
        acquireText.text = "<color=yellow>" + acquireItem.itemName + "</color>을(를) <color=yellow>" + acquireItemCount.ToString() + "</color>개 획득하셨습니다.";
        
        acquireLogCoroutine = StartCoroutine(AcquireLogTimer());

        yield return null;
    }

    private IEnumerator AcquireLogTimer()
    {
        yield return new WaitForSeconds(acquireLogRemainTime);
        acquireBaseUI.SetActive(false);
    }
}
