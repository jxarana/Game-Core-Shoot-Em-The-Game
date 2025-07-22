using UnityEngine;

public class tutorialTrigger : MonoBehaviour
{
    // Assign the tutorial panel to show when this trigger activates
    public GameObject tutorialPanel;

    // If true, this trigger will auto-activate at the start of the scene (e.g., movement prompt)
    public bool autoActivateOnStart = false;

    // Tracks whether the player is inside this trigger zone
    private bool playerInside = false;

    // Tracks whether the tutorial panel is currently shown
    private bool panelActive = false;

    void Start()
    {
        // If this trigger should activate at start (like movement tutorial), show the panel immediately
        if (autoActivateOnStart && tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            gameManager.instance.tutorial();
            panelActive = true;
            playerInside = true; // Enable input-based dismissal right away
        }
    }

    void Update()
    {
        // If the player is inside and panel is active, listen for input to dismiss
        if (playerInside && panelActive && Input.GetKeyDown(KeyCode.E))
        {
            tutorialPanel.SetActive(false);
            panelActive = false;
            gameManager.instance.stateUnpause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Activate the panel only if it isn't already active and the player enters the trigger
        if (!panelActive && other.CompareTag("Player"))
        {
            gameManager.instance.tutorial();
            panelActive = true;
            playerInside = true;
        }
    }
}