using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lion : StrongAnimal
{
    protected override void Update()
    {
        base.Update();
        if (theFieldOfViewAngle.View() && !isDead && !isAttacking)
        {
            StopAllCoroutines(); // 여러 코루틴이 동시에 실행되는 것을 방지
            StartCoroutine(ChaseTargetCoroutine());
        }
    }

    protected override void initAction()
    {
        base.initAction();
        RandomAction();
    }

    private void RandomAction()
    {
        RandomSound();

        int _random = Random.Range(0, 4); // 대기, 걷기

        if (_random == 0)
            Wait();
        else if (_random == 1)
            Eat();
        else if (_random == 2)
            Peek();
        else if (_random == 3)
            TryWalk();
    }

    private void Wait()
    {
        currentTime = waitTime;
    }

    protected void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
    }

    protected void Peek()
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
    }
}
