using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class DartControl : MonoBehaviour
{
    // References to game objects and components
    public GameObject DartPrefab;          // The dart prefab to spawn
    public Transform DartThrowPoint;       // Position where darts spawn
    ARSessionOrigin ARSession;             // Reference to AR session
    GameObject ARCam;                      // Reference to AR camera

    // Dart-related variables
    Transform db;                          // Dartboard transform
    private GameObject DartTemp;           // Currently active dart
    private Rigidbody rb;                 // Rigidbody of current dart

    // Throw control variables
    private bool canThrow = true;
    private float throwCooldown = 1f;      // Cooldown between throws

    // Distance and FPS tracking variables
    private bool isDBSearched = false;     // Has dartboard been found?
    private float dist = 0f;               // Distance to dartboard
    private float framespersec = 0f;       // Current FPS
    public TMP_Text text_dist;             // UI text for distance
    public TMP_Text fps;                   // UI text for FPS

    // FPS calculation variables
    private float timer = 0.0f;
    private float frames = 0.0f;
    private float waitTime = 1.0f;

    // Initialize AR components
    void Start()
    {
        ARSession = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        ARCam = ARSession.transform.Find("AR Camera").gameObject;
    }

    // Subscribe to dartboard placement event
    void OnEnable()
    {
        PlaceDartboard.onPlacedObject += DartsInit;
    }

    // Unsubscribe from dartboard placement event
    void OnDisable()
    {
        PlaceDartboard.onPlacedObject -= DartsInit;
    }

    void Update()
    {
        // Update FPS counter
        timer += Time.deltaTime;
        frames += 1;

        // Handle dart throwing
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && canThrow)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycasthit;
            if (Physics.Raycast(raycast, out raycasthit))
            {
                if (raycasthit.collider.CompareTag("dart"))
                {
                    // Prepare dart for throwing
                    raycasthit.collider.enabled = false;
                    DartTemp.transform.parent = ARSession.transform;

                    // Enable throwing force calculation
                    ThrowDart currentDartScript = DartTemp.GetComponent<ThrowDart>();
                    currentDartScript.isForceOK = true;

                    // Start throw cooldown and spawn new dart
                    StartCoroutine(ThrowCooldown());
                    DartsInit();
                }
            }
        }

        // Update distance to dartboard
        if (isDBSearched)
        {
            dist = Vector3.Distance(db.position, ARCam.transform.position);
            text_dist.text = dist.ToString().Substring(0, 3);
        }

        // Calculate and update FPS display
        if (timer > waitTime)
        {
            framespersec = frames / timer;
            timer -= waitTime;
            frames = 0.0f;
        }
        fps.text = framespersec.ToString();
    }

    // Initialize darts after dartboard placement
    void DartsInit()
    {
        db = GameObject.FindWithTag("dart_board").transform;
        if (db)
        {
            isDBSearched = true;
        }
        StartCoroutine(WaitAndSpawnDart());
    }

    // Spawn a new dart with slight delay
    public IEnumerator WaitAndSpawnDart()
    {
        yield return new WaitForSeconds(0.1f);
        DartTemp = Instantiate(DartPrefab, DartThrowPoint.position, ARCam.transform.localRotation);
        DartTemp.transform.parent = ARCam.transform;
        rb = DartTemp.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Implement throwing cooldown
    private IEnumerator ThrowCooldown()
    {
        canThrow = false;
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }
}