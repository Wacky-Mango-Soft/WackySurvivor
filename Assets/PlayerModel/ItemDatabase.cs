using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentItem {
    public GameObject prefab;
    public string name;
    public Vector3 tf_equipment;
    public Quaternion qt_equipment;
    public float localScale;
}

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] EquipmentItem[] equipmentItems;

    public EquipmentItem SearchEquipmentItem(string _name) {

        for (int i = 0; i < equipmentItems.Length; i++)
        {
            if (equipmentItems[i].name == _name) {
                Debug.Log("Item Founded : " + equipmentItems[i].name);
                return equipmentItems[i];
            }
        }
        Debug.LogWarning("Item with name " + _name + " not found.");
        return null;
    }

}
