using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    [SerializeField] GameObject attackCollision;
    SoundManager theSoundManager;

    public Animator Animator { get => animator; set => animator = value; }

    void Awake()
    {
        Animator = GetComponent<Animator>();
        theSoundManager = FindObjectOfType<SoundManager>();
    }

    public void OnAttackCollision()
    {
        attackCollision.SetActive(true);
    }

    public void OnMovement(float horizontal, float vertical) {
        Animator.SetFloat("horizontal", horizontal);
        Animator.SetFloat("vertical", vertical);
    }

    public void OnJump() {
        Animator.SetTrigger("Jump");
    }

    public void onPickup() {
        Animator.SetTrigger("Pickup");
    }

    public void onDodge() {
        Animator.SetTrigger("Roll");
    }

    public void onCrouch(bool isCrouch)
    {
        Animator.SetBool("Crouch", isCrouch);
    }

    public void OnAttack() {
        Animator.SetTrigger("Attack");
    }

    public void OnRun(bool isRun)
    {
        if (!GameManager.instance.isWater)
        { 
            // WeaponManager.currentWeaponAnim.SetBool("Run", isRun);
            Animator.SetBool("Run", isRun);
        }
    }

    public void OnAnimationSound(string soundName) {
        theSoundManager.PlaySE(soundName);
    }
}
