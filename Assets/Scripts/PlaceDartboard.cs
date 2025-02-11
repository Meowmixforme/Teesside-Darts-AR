using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceDartboard : MonoBehaviour
{
    public GameObject Object;
    public GameObject Placer;
    private Pose PlacerPose;
    private Transform PlacerTransform;
    private bool PoseValid = false;
    private bool isObjectPlaced = false;
    private TrackableId PlaneID = TrackableId.invalidId;
    private DartScoring dartScoring;
    private GameObject spawnedDartboard;

    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    public static event Action onPlacedObject;

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        dartScoring = FindObjectOfType<DartScoring>();
    }

    void Update()
    {
        if (!isObjectPlaced)
        {
            UpdatePlacementPosition();
            UpdatePlacementIndicator();

            if (PoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }
    }

    private void UpdatePlacementPosition()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        if (m_RaycastManager.Raycast(screenCenter, Hits, TrackableType.PlaneWithinPolygon))
        {
            PoseValid = Hits.Count > 0;
            if (PoseValid)
            {
                PlacerPose = Hits[0].pose;
                PlaneID = Hits[0].trackableId;

                var planeManager = GetComponent<ARPlaneManager>();
                ARPlane arPlane = planeManager.GetPlane(PlaneID);
                PlacerTransform = arPlane.transform;
            }
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (PoseValid)
        {
            Placer.SetActive(true);
            Placer.transform.SetPositionAndRotation(PlacerPose.position, PlacerTransform.rotation);
        }
        else
        {
            Placer.SetActive(false);
        }
    }

    private void PlaceObject()
    {
        spawnedDartboard = Instantiate(Object, PlacerPose.position, PlacerPose.rotation);

        // Setup dartboard for scoring
        if (dartScoring != null)
        {
            dartScoring.SetupDartboard(spawnedDartboard);
        }

        onPlacedObject?.Invoke();
        isObjectPlaced = true;
        Placer.SetActive(false);
    }
}