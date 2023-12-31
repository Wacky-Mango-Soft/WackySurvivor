using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle;  // 시야 각도 (130도)
    [SerializeField] private float viewDistance; // 시야 거리 (10미터)
    [SerializeField] private LayerMask targetMask;  // 타겟 마스크(플레이어)
    [SerializeField] private float showRayLength;

    //private Pig thePig;
    private PlayerController thePlayer;
    private NavMeshAgent nav;

    void Start()
    {
        //thePig = GetComponent<Pig>();
        thePlayer = FindObjectOfType<PlayerController>();
        nav = GetComponent<NavMeshAgent>();
    }

    public Vector3 GetTargetPos()
    {
        return thePlayer.transform.position;
    }

    // Deprecated
    //void Update()
    //{
    //    View();  // 매 프레임마다 시야 탐색
    //}

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    public bool View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 왼쪽으로 회전한 방향 (시야각의 왼쪽 경계선)
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);  // z 축 기준으로 시야 각도의 절반 각도만큼 오른쪽으로 회전한 방향 (시야각의 오른쪽 경계선)

        Debug.DrawRay(transform.position + transform.up, _leftBoundary * showRayLength, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary * showRayLength, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.name == "Player")
                        {
                            //Debug.Log("플레이어가 돼지 시야 내에 있습니다.");
                            Debug.DrawRay(transform.position + transform.up, _direction * showRayLength, Color.blue);
                            //thePig.Run(_hit.transform.position);
                            return true;
                        }
                    }
                }
            }

            if (thePlayer.GetRun())
            {
                if (CalcPathLength(thePlayer.transform.position) <= viewDistance)
                {
                    Debug.Log("주변에 뛰고 있는 플레이어의 움직임을 파악했습니다.");
                    return true;
                }
            }
        }
        return false;
    }

    // 현재 자신과 _targetPos 까지의 최단경로 거리 계산
    private float CalcPathLength(Vector3 _targetPos)
    {
        NavMeshPath _path = new NavMeshPath();
        nav.CalculatePath(_targetPos, _path);

        Vector3[] _wayPoint = new Vector3[_path.corners.Length + 2];

        _wayPoint[0] = transform.position;
        _wayPoint[_path.corners.Length + 1] = _targetPos;

        float _pathLength = 0;  // 경로 길이를 더함
        for (int i = 0; i < _path.corners.Length; i++)
        {
            _wayPoint[i + 1] = _path.corners[i];
            _pathLength += Vector3.Distance(_wayPoint[i], _wayPoint[i + 1]);
        }

        return _pathLength;
    }
}