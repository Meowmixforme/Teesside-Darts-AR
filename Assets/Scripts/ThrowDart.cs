using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ThrowDart : MonoBehaviour
{
    // Component references
    private Rigidbody rg;                  // Dart's rigidbody
    private GameObject d;                   // Spawn point reference
    private DartScoring dartScoring;        // Scoring system reference

    // State flags
    public bool isForceOK = false;         // Ready to apply throwing force
    bool isDartRotation = false;           // Dart is rotating during throw
    bool isDartReady = true;               // Dart is ready for initial rotation
    bool isDartOnBoard = false;            // Dart has hit the board

    // AR components
    ARSessionOrigin ARSession;             // AR session reference
    GameObject ARCam;                      // AR camera reference

    public Collider dartCollider;          // Dart's collider component

    // Initialise components and references
    void Start()
    {
        // Find AR components
        ARSession = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = ARSession.transform.Find("AR Camera").gameObject;

        // Get components
        rg = gameObject.GetComponent<Rigidbody>();
        d = GameObject.Find("Spawn");

        // Find scoring system
        dartScoring = FindObjectOfType<DartScoring>();
        if (dartScoring == null)
        {
            Debug.LogError("DartScoring component not found!");
        }
    }

    // Physics update
    private void FixedUpdate()
    {
        // Initialize dart throw
        if (isForceOK)
        {
            dartCollider.enabled = true;
            StartCoroutine(InitDartDestroyVFX());
            GetComponent<Rigidbody>().isKinematic = false;
            isForceOK = false;
            isDartRotation = true;
        }

        // Apply forward force to dart
        rg.AddForce(d.transform.forward * (12f + 6f) * Time.deltaTime, ForceMode.VelocityChange);

        // Handle initial slow rotation
        if (isDartReady)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 20f);
        }

        // Handle throw rotation
        if (isDartRotation)
        {
            isDartReady = false;
            transform.Rotate(Vector3.forward * Time.deltaTime * 400f);
        }
    }

    // Coroutine to destroy dart if it doesn't hit the board
    IEnumerator InitDartDestroyVFX()
    {
        yield return new WaitForSeconds(5f);
        if (!isDartOnBoard)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }

    // Handle dart collision with board
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dart_board"))
        {
            Handheld.Vibrate();                            // Provide haptic feedback

            // Stop dart movement
            GetComponent<Rigidbody>().isKinematic = true;
            isDartRotation = false;
            isDartOnBoard = true;

            // Calculate and apply score
            if (dartScoring != null)
            {
                dartScoring.ScoreDart(other);
            }
        }
    }
}
