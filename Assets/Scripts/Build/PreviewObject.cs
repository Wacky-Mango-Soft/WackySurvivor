using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Building.Type needType;
    private bool needTypeFlag;

    // 충돌한 오브젝트의 콜라이더
    private List<Collider> colliderList = new List<Collider>();

    [SerializeField] private int layerGround; // 지상 레이어
    private const int IGNORE_RAYCAST_LAYER = 2; // ignore raycast

    [SerializeField] private Material green;
    [SerializeField] private Material red;

    // Update is called once per frame
    void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if(needType == Building.Type.Normal)
        {
            if(colliderList.Count > 0)
                SetColor(red);
            else
                SetColor(green);
        }
        else
        {
            if (colliderList.Count > 0 || !needTypeFlag)
                SetColor(red);
            else
                SetColor(green);
        }
    }

    private void SetColor(Material mat)
    {
        // 자기 자신 안에 포함된 다른 오브젝트들의 트랜스폼을 줄줄히 꺼낼수 있음
        foreach(Transform tf_child in this.transform) {
            var newMaterials = new Material[tf_child.GetComponent<Renderer>().materials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = mat;
            }

            tf_child.GetComponent<Renderer>().materials = newMaterials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Structure")
        {
            if(other.GetComponent<Building>().type != needType)
                colliderList.Add(other);
            else
                needTypeFlag = true;
        }
        else
        {
            if(other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
                colliderList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Structure")
        {
            if (other.GetComponent<Building>().type != needType)
                colliderList.Remove(other);
            else
                needTypeFlag = false;
        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
                colliderList.Remove(other);
        }
    }

    public bool IsBuildable()
    {
        if(needType == Building.Type.Normal)
            return colliderList.Count == 0;
        else
            return colliderList.Count == 0 && needTypeFlag;
    }
}
