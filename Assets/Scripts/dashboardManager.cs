using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DashboardManager : MonoBehaviour
{
    public TextMeshProUGUI userIdText; // UI element to display the user ID and Username
    public GameObject friendsPanel;
    public GameObject gamesPanel;
    public GameObject achievementsPanel;
    public GameObject dashboardUI;
    public Button friendsButton;
    public Button gamesButton;
    public Button achievementsButton;
    public Button exitButton;
    public GameObject eventSystem;

    void Start()
    {
        eventSystem.SetActive(true);
        // Retrieve the user ID and Username from PlayerPrefs and display them
        int userID = PlayerPrefs.GetInt("UserID", -1); // Default to -1 if not found
        string username = PlayerPrefs.GetString("Username", "Guest");
        if (userID == -1 && username == "Guest")
        {
            Debug.Log("Unauthorized access attempt, not showing dashboard.");
            dashboardUI.SetActive(false);  // Deactivate the dashboard if default guest credentials are detected
        }
        else
        {
            dashboardUI.SetActive(true);   // Activate the dashboard if the user is properly authenticated
            userIdText.text = $"User ID: {userID} - Username: {username}";  // Update the text to show user details
        }
        // Initialize all panels to be hidden and dashboard shown
        friendsPanel.SetActive(false);
        gamesPanel.SetActive(false);
        achievementsPanel.SetActive(false);

        // Setup button listeners
        friendsButton.onClick.AddListener(() => ShowPanel(friendsPanel));
        gamesButton.onClick.AddListener(() => ShowPanel(gamesPanel));
        achievementsButton.onClick.AddListener(() => ShowPanel(achievementsPanel));
        exitButton.onClick.AddListener(ExitApplication);
    }

    // Function to show only the selected panel
    private void ShowPanel(GameObject panel)
    {
        // Hide all panels
        friendsPanel.SetActive(false);
        gamesPanel.SetActive(false);
        achievementsPanel.SetActive(false);

        // Show the selected panel
        panel.SetActive(true);
    }

    private void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Application closed"); // This will only show in the editor
    }
}