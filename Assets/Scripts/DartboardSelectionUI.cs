using UnityEngine;
using UnityEngine.UI;

public class DartboardSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private DartboardSpawner spawner;

    private void Start()
    {
        SetupDartboardButtons();
    }

    private void SetupDartboardButtons()
    {
        // Create buttons for each dartboard option
        var options = spawner.GetDartboardOptions();
        for (int i = 0; i < options.Length; i++)
        {
            int index = i; // Capture for lambda
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Set button image
            Image buttonImage = buttonObj.GetComponent<Image>();
            buttonImage.sprite = options[i].menuIcon;
            
            // Add click handler
            button.onClick.AddListener(() => {
                spawner.SpawnDartboard(index);
                HideSelectionMenu();
            });
        }
    }

    public void ShowSelectionMenu()
    {
        selectionPanel.SetActive(true);
    }

    public void HideSelectionMenu()
    {
        selectionPanel.SetActive(false);
    }
}