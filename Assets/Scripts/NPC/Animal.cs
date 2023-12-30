using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] protected string animalName; // 동물의 이름
    [SerializeField] protected int hp;  // 동물의 체력

    [SerializeField] protected float walkSpeed;  // 걷기 속력
    [SerializeField] protected float runSpeed;  // 달리기 속력
    //protected float applySpeed; #0 nav 사용으로 Rigidbody 우선순위 내려감. 적용안됨. 

    //protected Vector3 direction;  // 방향

    // 상태 변수
    protected bool isAction;  // 행동 중인지 아닌지 판별
    protected bool isWalking; // 걷는지, 안 걷는지 판별
    protected bool isRunning; // 달리는지 판별
    protected bool isDead;   // 죽었는지 판별
    protected bool isChasing; // 추격중인지 판별

    [SerializeField] protected float walkTime;  // 걷기 시간
    [SerializeField] protected float waitTime;  // 대기 시간
    [SerializeField] protected float runTime;  // 뛰기 시간
    [SerializeField] protected float movingDistance;  // 걸어가는 거리 || 플레이어로부터 도망치는 거리
    protected float currentTime;

    // 필요한 컴포넌트
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected FieldOfViewAngle theFieldOfViewAngle;

    [SerializeField] protected AudioClip[] sound_Normal; // 돼지의 일상 소리. 여러개라 배열로
    [SerializeField] protected AudioClip sound_Hurt;  // 돼지가 맞을 때 소리
    [SerializeField] protected AudioClip sound_Dead; // 돼지가 죽을 때 소리

    protected Vector3 destination;  // 목적지
    protected NavMeshAgent nav; // 필요한 컴포넌트

    void Start()
    {
        currentTime = waitTime;   // 대기 시작
        isAction = true;   // 대기도 행동
        theAudio = GetComponent<AudioSource>();  // 오디오 컴포넌트 가져오기
        nav = GetComponent<NavMeshAgent>();
        theFieldOfViewAngle = GetComponent<FieldOfViewAngle>();
    }

    protected virtual void Update() // 자식 객체에서도 사용하기 위해 가상함수 선언
    {
        if (!isDead)
        {
            Move();
            //Rotation(); #0
            ElapseTime();
        }
    }

    protected void Move()
    {
        if (isWalking || isRunning)
            nav.SetDestination(transform.position + destination * movingDistance);
            //rigid.MovePosition(transform.position + transform.forward * applySpeed * Time.deltaTime); #0
    }

    // Deprecated for navMesh
    //protected void Rotation()
    //{
    //    if (isWalking || isRunning)
    //    {
    //        Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), 0.01f); // y 회전값만 적용 (땅속이나 하늘로 도망가지 않게 하려면)
    //        rigid.MoveRotation(Quaternion.Euler(_rotation));
    //    }
    //}

    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0 && !isChasing)  // 랜덤하게 다음 행동을 개시 (추격중이 아닐때 추가)
                initAction();
        }
    }

    protected virtual void initAction()  // 다음 행동 준비
    {
        // 초기화
        isAction = true;
        nav.ResetPath(); // #0 네비 초기화
        isWalking = false;
        anim.SetBool("Walking", isWalking);
        isRunning = false;
        anim.SetBool("Running", isRunning);
        //applySpeed = walkSpeed; #0
        nav.speed = walkSpeed;

        // 랜덤한 방향으로 초기화
        //direction.Set(0f, Random.Range(0f, 360f), 0f);
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(0.5f, 1f));

        // 다음 행동 결정하기
        // RandomAction();
    }

    protected void TryWalk()  // 걷기
    {
        currentTime = walkTime;
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        nav.speed = walkSpeed;
        //Debug.Log("걷기");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Dead();
                return;
            }

            PlaySE(sound_Hurt);
            anim.SetTrigger("Hurt");
            // Run(_targetPos);
        }
    }

    protected void Dead()
    {
        PlaySE(sound_Dead);

        isWalking = false;
        isRunning = false;
        isDead = true;

        anim.SetTrigger("Dead");
    }

    protected void RandomSound()
    {
        int _random = Random.Range(0, sound_Normal.Length);
        PlaySE(sound_Normal[_random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    public bool GetIsDead()
    {
        return isDead;
    }
}
