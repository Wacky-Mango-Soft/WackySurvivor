using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ArchemyToolTip : MonoBehaviour
{
    [SerializeField] private Text txt_NeedItemName;
    [SerializeField] private Text txt_NeedItemNumber;

    [SerializeField] private GameObject go_BaseToolTip;

    private void Clear()
    {
        txt_NeedItemName.text = "";
        txt_NeedItemNumber.text = "";
    }

    public void ShowToolTip(string[] _needItemNames, int[] _needItemNumbers)
    {
        Clear();
        go_BaseToolTip.SetActive(true);

        for (int i = 0; i < _needItemNames.Length; i++)
        {
            txt_NeedItemName.text += _needItemNames[i] + "\n";
            txt_NeedItemNumber.text += "x " + _needItemNumbers[i] + "\n";
        }
    }

    public void HideTooltip()
    {
        Clear();
        go_BaseToolTip.SetActive(false);
    }

}
