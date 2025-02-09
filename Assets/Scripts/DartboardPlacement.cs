using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DartboardPlacement : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Vector3 initialScale;
    private bool isPlaced = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        initialScale = transform.localScale;

        // Start slightly smaller
        transform.localScale = initialScale * 0.8f;

        // Setup grab events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!isPlaced)
        {
            // Simple scale up
            transform.localScale = initialScale;
            isPlaced = true;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Ensure dartboard is vertical when released
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, currentRotation.y, 0);
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}