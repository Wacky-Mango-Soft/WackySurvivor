using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    private float applySpeed;   
    [SerializeField] private float jumpForce;

    // 상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

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

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();

        // 초기화
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrounch();
        Move();
        MoveCheck();
        CameraRotation();
        CharacterRotation();
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
        if (Input.GetKeyDown(KeyCode.Space) && isGround) {
            jump();
        }
    }
    
    // 점프
    private void jump() {
        if (isCrouch) {
            Crouch();
        }
        myRigid.velocity = transform.up * jumpForce;
    }

    // 달리기 시도
    private void TryRun() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
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
    private void CameraRotation() {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

}
