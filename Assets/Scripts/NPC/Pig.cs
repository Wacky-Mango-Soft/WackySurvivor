using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : WeakAnimal
{

    protected override void initAction()
    {
        base.initAction();
        RandomAction();
    }

    protected void RandomAction()
    {
        // 다음 행동을 결정하기에 앞서 랜덤하게 돼지의 일상 소리 재생 
        RandomSound();

        int _random = Random.Range(0, 4); // 대기, 풀뜯기, 두리번, 걷기

        if (_random == 0)
            Wait();
        else if (_random == 1)
            Eat();
        else if (_random == 2)
            Peek();
        else if (_random == 3)
            TryWalk();
    }

    protected void Wait()  // 대기
    {
        currentTime = waitTime;
        //Debug.Log("대기");
    }

    protected void Eat()  // 풀 뜯기
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        //Debug.Log("풀 뜯기");
    }

    protected void Peek()  // 두리번
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
        //Debug.Log("두리번");
    }
}