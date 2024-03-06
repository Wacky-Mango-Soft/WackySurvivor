using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    static public bool isActivated = true;

    // 스피드 조정 변수
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float swimSpeed;
    [SerializeField] private float swimFastSpeed;
    [SerializeField] private float upSwimSpeed;

    // #스피드 대입 변수 편한 디버깅을 위해 직렬화
    [SerializeField] private float applySpeed;   

    [SerializeField] private float jumpForce;

    // 상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    private bool isSwim = false;

    // 움직임 체크 변수
    private Vector3 lastPos;

    // 앉았을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // 카메라 민감도
    [SerializeField, Range(1, 10)] private float lookSensitivity;

    // 카메라 한계
    [SerializeField] private float cameraRotationLimit;
    private float currentCameraRotationX;

    // 컴포넌트
    [SerializeField] Camera theCamera;
    private Rigidbody myRigid;
    private CapsuleCollider capsuleCollider;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;

    [SerializeField] GameObject dieUI;
    [SerializeField] Text dieText;
    [SerializeField] SaveNLoad thesaveNLoad;

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType<StatusController>();

        // 초기화
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated && GameManager.instance.canPlayerMove)
        {
            WaterCheck();
            IsGround();
            TryJump();
            if (!GameManager.instance.isWater)
            {
                TryRun();
                TryCrounch();
            }
            Move();
            MoveCheck();
            CameraRotation();
            CharacterRotation();
        }
    }

    private void WaterCheck()
    {
        // #fix GetKeyDown 토글 형식에서 변경. 물에서 나온뒤 applySpeed가 swinSpeed 상태인 부분 수정. isSwim 추가.
        if (GameManager.instance.isWater)
        {
            isSwim = true;

            if (isCrouch)
            {
                Crouch();
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                applySpeed = swimFastSpeed;
            }
            else
            {
                applySpeed = swimSpeed;
            }

        } 
        else if (!GameManager.instance.isWater)
        {
            if (isSwim)
            {
                applySpeed = walkSpeed;
                isSwim = false;
            }
        }
    }


    // 앉기 시도
    private void TryCrounch() {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            Crouch();
        }
    }

    // 앉기 동작
    private void Crouch() {
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch) {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    // 부드러운 앉기 동작
    IEnumerator CrouchCoroutine() {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY) {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.1f);
            theCamera.transform.localPosition = new Vector3(0f, _posY, 0f);
            if (count > 30) { break; }
            yield return null;
        }

        theCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    // 지면 체크
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        theCrosshair.JumpingAnimation(!isGround);
    }

    // 점프 시도
    private void TryJump() {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.CurrentSp > 0 && !GameManager.instance.isWater) {
            jump();
        }
        else if (Input.GetKey(KeyCode.Space) && GameManager.instance.isWater)
        {
            UpSwim();
        }
    }

    private void UpSwim()
    {
        myRigid.velocity = transform.up * upSwimSpeed;
    }

    // 점프
    private void jump() {
        if (isCrouch) {
            Crouch();
        }
        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;
    }

    // 달리기 시도
    private void TryRun() {
        if (Input.GetKey(KeyCode.LeftShift) && theStatusController.CurrentSp > 0) {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.CurrentSp <= 0) {
            RunningCancle();
        }
    }

    // 달리기 실행
    private void Running() {
        if (isCrouch) {
            Crouch();
        }
        theGunController.CancelFineSight();
        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        theStatusController.DecreaseStamina(1);
        applySpeed = runSpeed;
    }

    // 달리기 취소
    private void RunningCancle() {
        isRun = false;
        applySpeed = walkSpeed;
        theCrosshair.RunningAnimation(isRun);
    }

    // 움직임 실행
    private void Move() {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        // #1 unity 2019 ver code. transform & rigidbody transform unmatched. fix needed.
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        // #1 fix
        transform.position = myRigid.position;
    }

    // 움직임 체크
    private void MoveCheck()
    {
        if (!isRun && !isCrouch && isGround)
        {
            if(Vector3.Distance(lastPos, transform.position) >= 0.01f)
                isWalk = true;
            else
                isWalk = false;

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
    }

    // 좌우 캐릭터 회전
    private void CharacterRotation() {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    // 상하 캐릭터 회전
    private bool pauseCameraRotation = false;

    private void CameraRotation() 
    {
        if (!pauseCameraRotation)
        {
            float _xRotation = Input.GetAxisRaw("Mouse Y");
            float _cameraRotationX = _xRotation * lookSensitivity;

            currentCameraRotationX -= _cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    public IEnumerator TreeLookCoroutine(Vector3 _target)
    {
        pauseCameraRotation = true;

        Quaternion direction = Quaternion.LookRotation(_target - theCamera.transform.position);
        Vector3 eulerValue = direction.eulerAngles;
        float destinationX = eulerValue.x;

        while(Mathf.Abs(destinationX - currentCameraRotationX) >= 0.5f)
        {
            eulerValue = Quaternion.Lerp(theCamera.transform.localRotation, direction, 0.3f).eulerAngles;  // 쿼터니언 👉 벡터
            theCamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f); // 벡터 👉 쿼터니언 (X축으로만 회전하면 됨)
            currentCameraRotationX = theCamera.transform.localEulerAngles.x; 
            yield return null;
        }
        pauseCameraRotation = false;
    }

    public bool GetRun()
    {
        return isRun;
    }

    public void Die() {
        if (!GameManager.instance.isDied) {
            GameManager.instance.isDied = true;
            //settriger "die"
            dieUI.SetActive(true);
            dieText.text = $"{TimeManager.instance.Day} Days Survive";
            Debug.Log("die실행");
            
        }
    }

   
}
