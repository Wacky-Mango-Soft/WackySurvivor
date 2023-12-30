using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private int hp; // 바위 체력
    [SerializeField] private int destroyTime; // 파편 제거 시간
    [SerializeField] private SphereCollider col; // 구체 컬라이더
    [SerializeField] private GameObject go_rock; // 일반 바위
    [SerializeField] private GameObject go_debris; // 깨진 바위
    [SerializeField] private GameObject go_effect_prefabs; // 채굴 이펙트

    [SerializeField]
    private GameObject go_rock_item_prefab; // 바위 파괴시 생성할 아이템 프리팹

    [SerializeField]
    private int count;  // 바위 파괴시 생성할 아이템 갯수

    // 필요한 사운드 이름
    [SerializeField] private string strike_Sound;
    [SerializeField] private string destroy_Sound;

    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        for (int i = 0; i < count; i++)
        {
            Instantiate(go_rock_item_prefab, go_rock.transform.position + (transform.up * 1f), Quaternion.identity);
        }

        col.enabled = false;
        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
    }
}
