using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private float gunAccuracy;
    [SerializeField] private GameObject go_CrosshairHUD;
    [SerializeField] private GunController theGunController;

    // 이동시 조준점 변경
    public void WalkingAnimation(bool _flag)
    {
        animator.SetBool("Walking", _flag);
    }
    public void RunningAnimation(bool _flag)
    {
        animator.SetBool("Running", _flag);
    }
    public void CrouchingAnimation(bool _flag)
    {
        animator.SetBool("Crouching", _flag);
    }
    public void FineSightAnimation(bool _flag)
    {
        animator.SetBool("FineSight", _flag);
    }

    // 사격시 조준점 변경
    public void FireAnimation()
    {
        if(animator.GetBool("Walking"))
        {
            animator.SetTrigger("Walk_Fire");
        }
        else if(animator.GetBool("Crouching"))
        {
            animator.SetTrigger("Crouch_Fire");
        }
        else
        {
            animator.SetTrigger("Idle_Fire");
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
}
