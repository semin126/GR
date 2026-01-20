using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class ARPlacementManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject previewPrefab;
    public GameObject calendarPrefab;
    public GameObject touchEffectPrefab;

    [Header("Settings")]
    public float holdTimeThreshold = 1.0f; // 1초 동안 누르면 설치됨

    private ARRaycastManager raycastManager;
    private GameObject spawnedPreview;
    private GameObject placedCalendar;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // 롱프레스를 위한 변수들
    private float touchStartTime;
    private bool isHolding = false;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        // 1. 현재 마우스/손가락 위치 파악
        Vector2 pointerPosition = Vector2.zero;
        bool isPressing = false;

        if (Pointer.current != null)
        {
            pointerPosition = Pointer.current.position.ReadValue();
            isPressing = Pointer.current.press.isPressed;
        }

        // 2. 레이캐스트 실행 (손가락 위치 기준)
        if (raycastManager.Raycast(pointerPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // 3. 미리보기(노란 사각형) 표시 및 이동
            if (placedCalendar == null)
            {
                if (spawnedPreview == null)
                    spawnedPreview = Instantiate(previewPrefab, hitPose.position, hitPose.rotation);

                spawnedPreview.transform.position = hitPose.position;
                spawnedPreview.transform.rotation = hitPose.rotation;
                spawnedPreview.SetActive(true);
            }

            // 4. 노란 원 이펙트 처리 (누르고 있을 때 손가락 위치에 생성)
            if (isPressing)
            {
                HandleTouchEffect(hitPose);
                HandleLongPress(hitPose);
            }
            else
            {
                isHolding = false; // 손을 떼면 타이머 초기화
            }
        }
        else
        {
            if (spawnedPreview != null) spawnedPreview.SetActive(false);
            isHolding = false;
        }
    }

    // 손가락 위치에 노란 원 생성 로직
    void HandleTouchEffect(Pose pose)
    {
        // 매 프레임 생성하면 너무 많으므로, 클릭 시작 시점에만 하나 생성하거나 
        // 혹은 누르는 동안 입자 효과처럼 보이게 조절 가능합니다.
        // 여기서는 '처음 눌렀을 때'만 생성되도록 구성하거나 짧은 주기로 생성합니다.
        if (Pointer.current.press.wasPressedThisFrame)
        {
            GameObject effect = Instantiate(touchEffectPrefab, pose.position + (pose.up * 0.01f), pose.rotation);
            Destroy(effect, 0.5f);
        }
    }

    // 길게 누르기 감지 로직
    void HandleLongPress(Pose pose)
    {
        if (Pointer.current.press.wasPressedThisFrame)
        {
            touchStartTime = Time.time;
            isHolding = true;
        }

        if (isHolding && placedCalendar == null)
        {
            float holdDuration = Time.time - touchStartTime;

            // 설정한 시간(예: 1초) 이상 눌렀다면 설치!
            if (holdDuration >= holdTimeThreshold)
            {
                placedCalendar = Instantiate(calendarPrefab, pose.position, pose.rotation);
                if (spawnedPreview != null) spawnedPreview.SetActive(false);
                isHolding = false; // 중복 설치 방지
                Debug.Log("길게 눌러서 달력 설치 완료!");
            }
        }
    }
}