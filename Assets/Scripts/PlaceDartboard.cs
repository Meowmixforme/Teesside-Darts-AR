using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceDartboard : MonoBehaviour
{
    // Object references
    public GameObject Object;              // Dartboard prefab to spawn
    public GameObject Placer;              // Visual indicator for placement
    private Pose PlacerPose;              // Current pose for placement
    private Transform PlacerTransform;     // Transform for placement orientation
    private bool PoseValid = false;        // Is current pose valid for placement
    private bool isObjectPlaced = false;   // Has dartboard been placed
    private TrackableId PlaneID = TrackableId.invalidId;  // ID of detected plane
    private DartScoring dartScoring;       // Reference to scoring system
    private GameObject spawnedDartboard;   // Instance of placed dartboard

    // AR components
    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    // Event to notify when object is placed
    public static event Action onPlacedObject;

    // Initialize components
    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        dartScoring = FindObjectOfType<DartScoring>();
    }

    // Update placement state
    void Update()
    {
        if (!isObjectPlaced)
        {
            UpdatePlacementPosition();     // Update possible placement position
            UpdatePlacementIndicator();    // Update visual indicator

            // Place object on touch if pose is valid
            if (PoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }
    }

    // Update placement position using AR raycast
    private void UpdatePlacementPosition()
    {
        // Raycast from screen center
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        if (m_RaycastManager.Raycast(screenCenter, Hits, TrackableType.PlaneWithinPolygon))
        {
            PoseValid = Hits.Count > 0;
            if (PoseValid)
            {
                PlacerPose = Hits[0].pose;
                PlaneID = Hits[0].trackableId;

                // Get plane orientation
                var planeManager = GetComponent<ARPlaneManager>();
                ARPlane arPlane = planeManager.GetPlane(PlaneID);
                PlacerTransform = arPlane.transform;
            }
        }
    }

    // Update visual placement indicator
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

    // Place dartboard in AR space
    private void PlaceObject()
    {
        // Instantiate dartboard
        spawnedDartboard = Instantiate(Object, PlacerPose.position, PlacerPose.rotation);

        // Initialize dartboard scoring
        if (dartScoring != null)
        {
            dartScoring.SetupDartboard(spawnedDartboard);
        }

        // Notify listeners and update state
        onPlacedObject?.Invoke();
        isObjectPlaced = true;
        Placer.SetActive(false);
    }
}