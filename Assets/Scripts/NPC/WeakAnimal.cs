using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakAnimal : Animal
{
    [SerializeField]
    protected Item item_Prefab;  // 해당 동물에게서 얻을 아이템
    [SerializeField]
    public int itemNumber;  // 아이템 획득 개수
    [SerializeField]
    protected float DeadBodyDisappearTime; // 시체가 사라지는데 걸리는 시간

    protected override void Update()  // #1 플레이어 발소리 인식 & 전방위 시야각 도주
    {
        base.Update();
        if (theFieldOfViewAngle.View() && !isDead)
        {
            Run(theFieldOfViewAngle.GetTargetPos());
        }
    }

    public void Run(Vector3 _targetPos)
    {
        //direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;
        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.z).normalized;
        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        //applySpeed = runSpeed;
        nav.speed = runSpeed;

        anim.SetBool("Running", isRunning);
    }

    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);

        if (!isDead)
        {
            Run(_targetPos);
        }
    }

    public Item GetItem()
    {
        this.gameObject.tag = "Untagged";
        Destroy(this.gameObject, DeadBodyDisappearTime);
        return item_Prefab;
    }
}
