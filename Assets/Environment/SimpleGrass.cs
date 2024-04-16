using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGrass : MonoBehaviour
{
    [SerializeField] private int hp; // 풀 체력. 공격 X 액션컨트롤러 통해 E키로 채집.
    [SerializeField] private GameObject go_effect_prefabs; // 채집 이펙트
    [SerializeField] private float effectDestroyTime; // 이펙트 제거 시간
    [SerializeField] private Item acquireItem; // 얻게되는 재료
    [SerializeField] private int acquireItemMaxCount; // 1회 채집시 얻는 재료 갯수 (1개 ~ acquireItemCount)
    [SerializeField] private string gathering_sound; // 채집 소리
    private Inventory theInventory;
    public int Hp { get => hp; set => hp = value; }

    // Start is called before the first frame update
    void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
    }

    public void Gathering() {
        SoundManager.instance.PlaySE(gathering_sound);
        var clone = Instantiate(go_effect_prefabs, GetComponent<BoxCollider>().bounds.center, Quaternion.identity);
        Destroy(clone, effectDestroyTime);

        int acquireItemCount = Random.Range(1, acquireItemMaxCount + 1);

        theInventory.AcquireItem(acquireItem, acquireItemCount);
        StartCoroutine(AcquirerItem.instance.AcquireLogCoroutine(acquireItem, acquireItemCount));

        Hp--;

        if (Hp <= 0)
            Destroy(gameObject);
    }

    
}
