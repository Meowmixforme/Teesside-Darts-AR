using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DartboardSpawner : MonoBehaviour
{
    [System.Serializable]
    public class DartboardOption
    {
        public string name;
        public GameObject prefab;
        public Sprite menuIcon;
        [TextArea]
        public string description; // Optional: for showing dartboard info
    }

    [Header("Dartboard Options")]
    [SerializeField] private DartboardOption[] dartboardOptions;

    [Header("AR Components")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private XRInteractionManager interactionManager;

    [Header("UI References")]
    [SerializeField] private Transform selectionPanel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI infoText;

    private GameObject spawnedDartboard;
    private ARPlaneManager planeManager;

    private void Awake()
    {
        raycastManager = FindAnyObjectByType<ARRaycastManager>();
        planeManager = FindAnyObjectByType<ARPlaneManager>();
        interactionManager = FindAnyObjectByType<XRInteractionManager>();

        SetupSelectionButtons();
    }

    // Add this method back in
    public DartboardOption[] GetDartboardOptions()
    {
        return dartboardOptions;
    }

    private void SetupSelectionButtons()
    {
        // Clear existing buttons
        foreach (Transform child in selectionPanel)
        {
            Destroy(child.gameObject);
        }

        // Create new buttons for each dartboard
        for (int i = 0; i < dartboardOptions.Length; i++)
        {
            int index = i; // Capture for lambda
            GameObject buttonObj = Instantiate(buttonPrefab, selectionPanel);

            // Get button components
            var button = buttonObj.GetComponent<UnityEngine.UI.Button>();
            var iconImage = buttonObj.transform.Find("Icon")?.GetComponent<UnityEngine.UI.Image>();
            var nameText = buttonObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();

            // Set button properties
            if (iconImage != null) iconImage.sprite = dartboardOptions[i].menuIcon;
            if (nameText != null) nameText.text = dartboardOptions[i].name;

            // Add click handler
            button.onClick.AddListener(() => SpawnDartboard(index));
        }
    }

    public void SpawnDartboard(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= dartboardOptions.Length) return;

        // If there's already a dartboard, destroy it
        if (spawnedDartboard != null)
        {
            Destroy(spawnedDartboard);
        }

        // Create new dartboard at camera position
        var cameraTransform = Camera.main.transform;
        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * 2f;
        spawnedDartboard = Instantiate(dartboardOptions[optionIndex].prefab, spawnPosition, Quaternion.identity);

        // Update info text if available
        if (infoText != null)
        {
            infoText.text = $"Selected: {dartboardOptions[optionIndex].name}\n{dartboardOptions[optionIndex].description}";
        }
    }

    public void DeleteCurrentDartboard()
    {
        if (spawnedDartboard != null)
        {
            Destroy(spawnedDartboard);
            spawnedDartboard = null;

            if (infoText != null)
            {
                infoText.text = "No dartboard selected";
            }
        }
    }
}