using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ThrowDart : MonoBehaviour
{
    private Rigidbody rg;
    private GameObject d;
    public bool isForceOK = false;
    bool isDartRotation = false;
    bool isDartReady = true;
    bool isDartOnBoard = false;

    ARSessionOrigin ARSession;
    GameObject ARCam;

    public Collider dartCollider;
    private DartScoring dartScoring; // Add this

    void Start()
    {
        ARSession = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = ARSession.transform.Find("AR Camera").gameObject;

        rg = gameObject.GetComponent<Rigidbody>();
        d = GameObject.Find("Spawn");

        // Add this to find DartScoring
        dartScoring = FindObjectOfType<DartScoring>();
        if (dartScoring == null)
        {
            Debug.LogError("DartScoring component not found!");
        }
    }

    private void FixedUpdate()
    {
        if (isForceOK)
        {
            dartCollider.enabled = true;
            StartCoroutine(InitDartDestroyVFX());
            GetComponent<Rigidbody>().isKinematic = false;
            isForceOK = false;
            isDartRotation = true;
        }

        rg.AddForce(d.transform.forward * (12f + 6f) * Time.deltaTime, ForceMode.VelocityChange);

        if (isDartReady)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 20f);
        }

        if (isDartRotation)
        {
            isDartReady = false;
            transform.Rotate(Vector3.forward * Time.deltaTime * 400f);
        }
    }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dart_board"))
        {
            Handheld.Vibrate();

            GetComponent<Rigidbody>().isKinematic = true;
            isDartRotation = false;
            isDartOnBoard = true;

            // Add scoring
            if (dartScoring != null)
            {
                dartScoring.ScoreDart(other);
            }
        }
    }
}
