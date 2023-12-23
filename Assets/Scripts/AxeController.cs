using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
        // 활성화 여부
    public static bool isActivate = true;

    private void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
    }

    void Update()
    {
        if (isActivate)
        {
            TryAttack();
        }
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                if (hitInfo.transform.tag == "Grass")
                {
                    hitInfo.transform.GetComponent<Grass>().Damage();
                }
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }
    
    public override void CloseWeaponChange(CloseWeapon _CloseWeapon)
    { 
        base.CloseWeaponChange(_CloseWeapon);
        isActivate = true;
    }
}
