using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : CloseWeaponController
{
    // 활성화 여부
    public static bool isActivate = false;

    void Update()
    {
        if (isActivate && GameManager.instance.canPlayerMove)
        {
            TryAttack();
        }
    }

    // Deprecated
    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                if (hitInfo.transform.tag == "Twig")
                {
                    hitInfo.transform.GetComponent<Twig>().Damage(this.transform);
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
        ComboAttackTrigger(true);
        isActivate = true;
    }

    public void ComboAttackTrigger(bool state) {
        WeaponManager.thePlayerAnimator.Animator.SetBool("isWeapon", state);
    }
}
