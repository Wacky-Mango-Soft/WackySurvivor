using System.Collections;
using UnityEngine;

public class PickaxeController : CloseWeaponController
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

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                if(hitInfo.transform.CompareTag("Rock"))
                {
                    hitInfo.transform.GetComponent<Rock>().Mining();
                }
                else if (hitInfo.transform.CompareTag("Twig"))
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
        isActivate = true;
    }
}
