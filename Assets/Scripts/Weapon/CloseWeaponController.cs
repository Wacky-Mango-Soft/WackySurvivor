using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 미완성 추상 클레스
public abstract class CloseWeaponController : MonoBehaviour
{

    // 현재 장착된 Hand형 타입 무기
    [SerializeField] protected CloseWeapon currentCloseWeapon;

    // 공격 상태 변수
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    [SerializeField] protected LayerMask layerMask;

    // 필요한 컴포넌트
    protected PlayerController thePlayerController;

    private ActionController actionController;

    private void Awake()
    {
        thePlayerController = FindObjectOfType<PlayerController>();
        actionController = FindObjectOfType<ActionController>();
    }

    protected void TryAttack()
    {
        if (!Inventory.invectoryActivated)
        {
            if (Input.GetButton("Fire1"))
            {
                if (!isAttack)
                {
                    if (CheckObject())
                    {
                        if (currentCloseWeapon.isAxe && hitInfo.transform.tag == "Tree")
                        {
                            //Debug.Log(hitInfo.transform.tag + "주시중");
                            StartCoroutine(thePlayerController.TreeLookCoroutine(hitInfo.transform.GetComponent<TreeComponent>().GetTreeCenterPosition()));
                            StartCoroutine(AttackCoroutine("Attack", currentCloseWeapon.workDelayA, currentCloseWeapon.workDelayB, currentCloseWeapon.workDelay));
                            return;
                        }
                        else if (hitInfo.transform.tag == "Weak_Animal" || hitInfo.transform.tag == "Strong_Animal") // #1 근접 무기 뭘로 때려도 NPC 타격
                        {
                            SoundManager.instance.PlaySE("Animal_Hit");
                            hitInfo.transform.GetComponent<Animal>().Damage(currentCloseWeapon.damage, transform.position);
                        }
                    }

                    // 공격 애니메이션 선택 설정부

                    if (currentCloseWeapon.isWeapon || currentCloseWeapon.isHand) {
                        isAttack = true;
                        StartCoroutine(AnimDependentAttack("ComboAttack", currentCloseWeapon.closeWeaponName));
                    }
                    else {
                        StartCoroutine(AttackCoroutine("Attack", currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
                    }

                }
            }
        }
    }

    // 작업 능력에 따른 공격 코루틴
    protected IEnumerator AttackCoroutine(string swingType, float _delayA, float _delayB, float _delay)
    {
        isAttack = true;
        WeaponManager.thePlayerAnimator.Animator.SetTrigger(swingType);

        yield return new WaitForSeconds(_delayA);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(_delayB);
        isSwing = false;

        yield return new WaitForSeconds(_delay - _delayA - _delayB);
        isAttack = false;
    }

    // 애니메이션 트리거 이벤트에 따른 공격 코루틴
    protected IEnumerator AnimDependentAttack(string swingType, string currentWeaponName) {

        switch (currentWeaponName)
        {
            case "Unarmed":
                // SoundManager.instance.PlaySE("Punch");
                break;
            case "TwohandSword":
                // SoundManager.instance.PlaySE("Sword_swing");
                break;
        }

        WeaponManager.thePlayerAnimator.Animator.SetTrigger(swingType);

        // yield return new WaitForSeconds(WeaponManager.thePlayerAnimator.Animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.5f);

        isAttack = false;
    }

    // 미완성 추상 코루틴
    protected abstract IEnumerator HitCoroutine();

    protected bool CheckObject()
    {
        if (Physics.Raycast(actionController.Tf_rayStartPoint, actionController.CurrentCameraForward, out hitInfo, actionController.CurrentRaycastRange - actionController.Range + currentCloseWeapon.range, layerMask))
        {
            return true;
        }
        return false;
    }

    // 가상 함수
    public virtual void CloseWeaponChange(CloseWeapon _CloseWeapon)
    {
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }
        currentCloseWeapon = _CloseWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
        WeaponManager.thePlayerAnimator.Animator.runtimeAnimatorController = WeaponManager.currentWeaponAnim.runtimeAnimatorController;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}
