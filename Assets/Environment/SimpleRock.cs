using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRock : MonoBehaviour
{
    [SerializeField] Material capMaterial;
    [SerializeField] private int hp; // 바위 체력
    [SerializeField] private int destroyTime; // 파편 제거 시간
    [SerializeField] private GameObject go_rock_item_prefab; // 바위 파괴시 생성할 아이템 프리팹
    [SerializeField] private GameObject go_effect_prefabs; // 채굴 이펙트
    [SerializeField] private Item acquireItem; // 얻게되는 재료
    [SerializeField] private int acquireItemMaxCount; // 바위 1회 가격시 얻는 재료 갯수 (1개 ~ acquireItemCount)
    [SerializeField] private int count;  // 바위 파괴시 생성할 아이템 갯수

    // 필요한 사운드 이름
    [SerializeField] private string strike_Sound;
    [SerializeField] private string destroy_Sound;

    private Inventory theInventory;
    
    public void Start() {
        theInventory = FindObjectOfType<Inventory>();
    }

    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);
        var clone = Instantiate(go_effect_prefabs, GetComponent<MeshCollider>().bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        int acquireItemCount = Random.Range(1, acquireItemMaxCount + 1);

        theInventory.AcquireItem(acquireItem, acquireItemCount);
        StartCoroutine(AcquirerItem.instance.AcquireLogCoroutine(acquireItem, acquireItemCount));

        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        for (int i = 0; i < count; i++)
        {
            Instantiate(go_rock_item_prefab, transform.position + (transform.up * 1f), Quaternion.identity);
        }

        GameObject[] rockDebris = MeshCut.Cut(gameObject, transform.position, Vector3.left, capMaterial);
        MeshCollider meshColider = rockDebris[1].AddComponent<MeshCollider>();
        meshColider.convex = true;

        foreach(GameObject obj in rockDebris) {
            obj.AddComponent<Rigidbody>();
            Destroy(obj, destroyTime);
        }
    }
}
