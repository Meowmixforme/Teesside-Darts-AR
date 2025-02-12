using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

// Ensures the ARPlaneManager component is present on the GameObject
[RequireComponent(typeof(ARPlaneManager))]

public class ARPlaneController : MonoBehaviour
{
    // Reference to the AR Plane Manager component
    ARPlaneManager m_ARPlaneManager;

    // Initialise the AR Plane Manager reference when the object is created
    void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Register to the dartboard placement event when this component is enabled
    void OnEnable()
    {
        PlaceDartboard.onPlacedObject += DisablePlaneDetection;
    }

    // Unregister from the dartboard placement event when this component is disabled
    void OnDisable()
    {
        PlaceDartboard.onPlacedObject -= DisablePlaneDetection;
    }

    // Called when the dartboard is placed
    // Disables plane visualisation and plane detection
    void DisablePlaneDetection()
    {
        SetAllPlanesActive(false);  // Hide all detected planes
        m_ARPlaneManager.enabled = !m_ARPlaneManager.enabled;  // Toggle plane manager off
    }

    // Helper method to show/hide all detected AR planes
    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }
}
