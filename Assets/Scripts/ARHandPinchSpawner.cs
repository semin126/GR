using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.ARFoundation;


public class ARHandPinchSpawner : MonoBehaviour
{
    public GameObject objectToPlace; // 생성할 큐브 프리팹
    private XRHandManager handManager;
    private bool isPinching = false;

    void Start() => handManager = GetComponent<XRHandManager>();

    void Update()
    {
        // 오른손 데이터 확인
        var rightHand = handManager.rightHand;
        if (!rightHand.isTracked) return;

        // 핀치(집기) 강도가 0.8 이상이면 실행
        if (rightHand.pinchStrength > 0.8f)
        {
            if (!isPinching) // 처음 집었을 때 딱 한 번만 생성
            {
                isPinching = true;
                SpawnAtIndexTip(rightHand);
            }
        }
        else if (rightHand.pinchStrength < 0.5f)
        {
            isPinching = false; // 손을 떼면 다시 생성 가능 상태로
        }
    }

    void SpawnAtIndexTip(XRHand hand)
    {
        // 검지 끝(Index Tip)의 현재 위치와 회전값 가져오기
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose pose))
        {
            // 손가락 끝 위치에 큐브 생성
            Instantiate(objectToPlace, pose.position, pose.rotation);
        }
    }
}