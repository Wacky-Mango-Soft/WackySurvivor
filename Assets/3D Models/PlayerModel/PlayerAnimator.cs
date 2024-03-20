using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimatorControllerSet {
    public string name;
    public RuntimeAnimatorController runtimeController;
    public EquipState equipState;
}

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private AnimatorControllerSet[] animators;
    private Animator selectedAnimator;
    private AnimatorControllerSet currentAnimatorSet;
    [SerializeField] GameObject attackCollision;

    void Awake()
    {
        selectedAnimator = GetComponent<Animator>();
        currentAnimatorSet = animators[0];
        selectedAnimator.runtimeAnimatorController = currentAnimatorSet.runtimeController;
    }

    void AnimatorChanger(EquipState equipState) { 
        switch (equipState)
        {
            case EquipState.UNARMED:
                currentAnimatorSet = animators[0];
                selectedAnimator.runtimeAnimatorController = currentAnimatorSet.runtimeController;
            break;
            case EquipState.TOOLEQUIPMENT:
                currentAnimatorSet = animators[1];
                selectedAnimator.runtimeAnimatorController = currentAnimatorSet.runtimeController;
            break;
            case EquipState.TWOHANDED:
                currentAnimatorSet = animators[2];
                selectedAnimator.runtimeAnimatorController = currentAnimatorSet.runtimeController;
            break;

        }
    }

    public void OnMovement(float horizontal, float vertical) {
        selectedAnimator.SetFloat("horizontal", horizontal);
        selectedAnimator.SetFloat("vertical", vertical);
    }

    public void OnJump() {
        selectedAnimator.SetTrigger("Jump");
    }

    public void OnAttack()
    {
        EquipState currentEquipState = currentAnimatorSet.equipState;

        switch(currentEquipState) {
            case EquipState.UNARMED:
                selectedAnimator.SetTrigger("Attack");
                break;
            case EquipState.TOOLEQUIPMENT:
                selectedAnimator.SetTrigger("onPickaxeWorking");
                break;
            case EquipState.TWOHANDED:
                selectedAnimator.SetTrigger("Attack");
                break;
        }
    }

    public void onPickup() {
        selectedAnimator.SetTrigger("Pickup");
    }

    public void onDodge() {
        selectedAnimator.SetTrigger("Roll");
    }

    public void OnAttackCollision()
    {
        attackCollision.SetActive(true);
    }
}
