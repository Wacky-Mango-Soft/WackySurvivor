using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private float gunAccuracy;
    [SerializeField] private GameObject go_CrosshairHUD;
    [SerializeField] private GunController theGunController;
    [SerializeField] private GameObject[] obj_crosshairs;

    // 이동시 조준점 변경
    public void WalkingAnimation(bool _flag)
    {
        if (!GameManager.instance.isWater)
        {
            // WeaponManager.currentWeaponAnim.SetBool("Walk", _flag);
            animator.SetBool("Walking", _flag);
        }
    }
    public void RunningAnimation(bool _flag)
    {
        if (!GameManager.instance.isWater)
        { 
            WeaponManager.currentWeaponAnim.SetBool("Run", _flag);
            animator.SetBool("Running", _flag);
        }
    }
    public void JumpingAnimation(bool _flag)
    {
        if (!GameManager.instance.isWater)
        {
            animator.SetBool("Running", _flag);
        }
    }
    public void CrouchingAnimation(bool _flag)
    {
        if (!GameManager.instance.isWater)
        {
            animator.SetBool("Crouching", _flag);
        }
    }
    public void FineSightAnimation(bool _flag)
    {
        if (!GameManager.instance.isWater)
        {
            animator.SetBool("FineSight", _flag);
        }
    }

    // 사격시 조준점 변경
    public void FireAnimation()
    {
        if (!GameManager.instance.isWater)
        {
            if (animator.GetBool("Walking"))
            {
                animator.SetTrigger("Walk_Fire");
            }
            else if (animator.GetBool("Crouching"))
            {
                animator.SetTrigger("Crouch_Fire");
            }
            else
            {
                animator.SetTrigger("Idle_Fire");
            }
        }
    }

    // 조준율 변경
    public float GetAccuracy()
    {
        if (animator.GetBool("Walking"))
        {
            gunAccuracy = 0.06f;
        }
        else if (animator.GetBool("Crouching"))
        {
            gunAccuracy = 0.015f;
        }
        else if (theGunController.GetFineSightMode())
        {
            gunAccuracy = 0.001f;
        }
        else
        {
            gunAccuracy = 0.035f;
        }
        return gunAccuracy;
    }

    public void PersonViewModeChanger(string _state)
    {
        if (_state == "OnePerson") {
            for (int i = 1; i < obj_crosshairs.Length; i++)
            {
                obj_crosshairs[i].SetActive(true);
            }
        }
        else if (_state == "ThirdPerson") {
            for (int i = 1; i < obj_crosshairs.Length; i++)
            {
                obj_crosshairs[i].SetActive(false);
            }
        }
    }
}
