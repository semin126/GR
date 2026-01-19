using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTouchCreator : MonoBehaviour
{
    public GameObject objectToPlace; // 큐브 프리팹을 여기에 드래그
    private ARRaycastManager raycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start() => raycastManager = GetComponent<ARRaycastManager>();

    void Update()
    {
        if (Input.touchCount == 0) return;

        if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
        }
    }
}