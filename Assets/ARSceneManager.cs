using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ARSceneManager : MonoBehaviour
{
    public enum AppState { Normal, Placement, Edit }
    public AppState currentState = AppState.Normal;

    [Header("UI Groups")]
    public GameObject addBtn;
    public GameObject placementGroup;
    public GameObject editGroup;
    public GameObject cancelBtn;

    [Header("AR References")]
    public ARPlaneManager planeManager;
    public ARRaycastManager raycastManager;

    [Header("Prefabs (Assets)")]
    public GameObject calendarPreviewPrefab;
    public GameObject calendarPrefab;
    public GameObject touchEffectPrefab;

    private GameObject spawnedPreview;
    private GameObject spawnedTouchEffect;
    private GameObject selectedCalendar;

    // 이동 취소를 위한 위치/회전값 저장 변수
    private Vector3 originalPos;
    private Quaternion originalRot;

    private Vector3 liftDir;
    private const float LiftHeight = 0.15f;
    private float touchTime = 0;

    void Start()
    {
        if (calendarPreviewPrefab != null)
        {
            spawnedPreview = Instantiate(calendarPreviewPrefab);
            spawnedPreview.SetActive(false);
            if (spawnedPreview.TryGetComponent<Collider>(out Collider c)) Destroy(c);
        }
        if (touchEffectPrefab != null)
        {
            spawnedTouchEffect = Instantiate(touchEffectPrefab);
            spawnedTouchEffect.SetActive(false);
            if (spawnedTouchEffect.TryGetComponent<Collider>(out Collider c)) Destroy(c);
        }
        SetMode(AppState.Normal);
    }

    void Update()
    {
        UpdateTouchCursor();

        if (currentState == AppState.Placement) HandlePlacementInput();
        if (currentState == AppState.Normal) HandleLongClick();
    }

    void UpdateTouchCursor()
    {
        if (spawnedTouchEffect == null) return;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 pointerPos = Pointer.current.position.ReadValue();

        if (raycastManager.Raycast(pointerPos, hits, TrackableType.PlaneWithinPolygon))
        {
            spawnedTouchEffect.SetActive(true);
            spawnedTouchEffect.transform.SetPositionAndRotation(hits[0].pose.position, hits[0].pose.rotation);
        }
        else
        {
            spawnedTouchEffect.SetActive(false);
        }
    }

    public void SetMode(AppState newState)
    {
        currentState = newState;
        addBtn.SetActive(newState == AppState.Normal);
        placementGroup.SetActive(newState == AppState.Placement);
        editGroup.SetActive(newState == AppState.Edit);
        cancelBtn.SetActive(newState != AppState.Normal);

        if (newState != AppState.Placement)
        {
            if (spawnedPreview) spawnedPreview.SetActive(false);
        }

        TogglePlaneVisibility(newState != AppState.Normal);
    }

    void HandlePlacementInput()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            if (spawnedPreview)
            {
                spawnedPreview.SetActive(true);
                spawnedPreview.transform.SetPositionAndRotation(hits[0].pose.position, hits[0].pose.rotation);
            }

            // 이동 중일 때 선택된 달력이 미리보기를 따라다님
            if (selectedCalendar != null)
            {
                selectedCalendar.transform.SetPositionAndRotation(hits[0].pose.position, hits[0].pose.rotation);
            }
        }
        else
        {
            if (spawnedPreview) spawnedPreview.SetActive(false);
        }
    }

    public void OnClickConfirm()
    {
        if (selectedCalendar == null)
        {
            if (spawnedPreview != null && spawnedPreview.activeSelf)
            {
                GameObject newObj = Instantiate(calendarPrefab, spawnedPreview.transform.position, spawnedPreview.transform.rotation);
                newObj.tag = "Calendar";
                SetMode(AppState.Normal);
            }
        }
        else
        {
            selectedCalendar = null;
            SetMode(AppState.Normal);
        }
    }

    public void OnClickAdd()
    {
        selectedCalendar = null;
        SetMode(AppState.Placement);
    }

    public void OnClickDelete()
    {
        if (selectedCalendar) Destroy(selectedCalendar);
        selectedCalendar = null;
        SetMode(AppState.Normal);
    }

    public void OnClickCancel()
    {
        if (selectedCalendar != null)
        {
            // [수정] 이동 중 혹은 편집 중에 취소하면 저장해둔 원래 위치로 복구
            selectedCalendar.transform.SetPositionAndRotation(originalPos, originalRot);
        }
        selectedCalendar = null;
        SetMode(AppState.Normal);
    }

    public void OnClickMove()
    {
        if (selectedCalendar != null)
        {
            // 이동 시작 전, 들려있는 상태가 아닌 "원래 바닥 위치"를 저장해야 함
            // 롱클릭 시 LiftHeight만큼 올렸으므로 그걸 뺀 위치를 저장
            originalPos = selectedCalendar.transform.position - (liftDir * LiftHeight);
            originalRot = selectedCalendar.transform.rotation;

            SetMode(AppState.Placement);
        }
    }

    void HandleLongClick()
    {
        if (Pointer.current == null || !Pointer.current.press.isPressed) { touchTime = 0; return; }
        touchTime += Time.deltaTime;
        if (touchTime > 0.8f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Calendar"))
                {
                    selectedCalendar = hit.collider.gameObject;
                    liftDir = hit.normal;

                    // 롱클릭 시점의 원래 위치 저장 (Move 안 누르고 바로 Cancel 할 경우 대비)
                    originalPos = selectedCalendar.transform.position;
                    originalRot = selectedCalendar.transform.rotation;

                    selectedCalendar.transform.position += liftDir * LiftHeight;
                    SetMode(AppState.Edit);
                    touchTime = 0;
                }
            }
        }
    }

    void TogglePlaneVisibility(bool visible)
    {
        if (!planeManager) return;
        planeManager.enabled = visible;
        foreach (var plane in planeManager.trackables) plane.gameObject.SetActive(visible);
    }
}