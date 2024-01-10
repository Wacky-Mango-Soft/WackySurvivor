using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponManager : MonoBehaviour
{
    // 무기 중복 교체 실행 방지
    public static bool isChangeWeapon = false;

    // 현재 무기와 현재 무기의 애니메이션
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    // 현재 무기의 타입
    [SerializeField] private string currentWeaponType;

    // 무기 교체 딜레이
    [SerializeField] private float changeWeaponDelayTime;
    [SerializeField] private float changeWeaponEndDelayTime;

    // 무기 배열
    [SerializeField] Gun[] guns;
    [SerializeField] CloseWeapon[] hands;
    [SerializeField] CloseWeapon[] axes;
    [SerializeField] CloseWeapon[] pickaxes;

    // 무기 배열을 문자열로 분류할 딕셔너리
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();

    // 필요한 컴포넌트
    [SerializeField] private GunController theGunController;
    [SerializeField] private HandController theHandController;
    [SerializeField] private AxeController theAxeController;
    [SerializeField] private PickaxeController thePickaxeController;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        }        
        for (int i = 0; i < axes.Length; i++)
        {
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        }        
        for (int i = 0; i < pickaxes.Length; i++)
        {
            pickaxeDictionary.Add(pickaxes[i].closeWeaponName, pickaxes[i]);
        }

    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;
    }

    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight();
                theGunController.CancelReload();
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                if (HandController.currentKit != null)
                    theHandController.Cancel();
                if (QuickSlotController.go_HandItem != null)
                    Destroy(QuickSlotController.go_HandItem);
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;            
            case "PICKAXE":
                PickaxeController.isActivate = false;
                break;
        }
    }

    private void WeaponChange(string _type, string _name)
    {
        if (_type == "GUN")
        {
            theGunController.GunChange(gunDictionary[_name]);
        }
        else if (_type == "HAND")
        {
            theHandController.CloseWeaponChange(handDictionary[_name]);
        }
        else if (_type == "AXE")
        {
            theAxeController.CloseWeaponChange(axeDictionary[_name]);
        }        
        else if (_type == "PICKAXE")
        {
            thePickaxeController.CloseWeaponChange(pickaxeDictionary[_name]);
        }
    }

    public IEnumerator WeaponInCoroutine()
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);
        
        currentWeapon.gameObject.SetActive(false);
    }

    public void WeaponOut()
    {
        isChangeWeapon = false;
        currentWeapon.gameObject.SetActive(true);
    }
}
