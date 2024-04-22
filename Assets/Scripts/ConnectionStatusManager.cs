using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ConnectionStatusManager : MonoBehaviour
{
    public TextMeshProUGUI connectionStatusText;
    public GameObject connectionPanel;
    public Button retryButton;  // Reference to the retry button
    public GameObject loginCanvas;  // Reference to the Login Canvas GameObject
    public GameObject registrationCanvas;  // Reference to the Registration Canvas GameObject

    void Start()
    {
        retryButton.gameObject.SetActive(false); // Initially hide the retry button
        retryButton.onClick.AddListener(() => CheckConnection("retry"));  // Add retry listener
        CheckConnection("initial");  // Perform an initial connection check
    }

    public void CheckConnection(string context)
    {
        connectionPanel.SetActive(true);  // Show the connection status panel
        connectionStatusText.text = "Checking Connection...";
        retryButton.gameObject.SetActive(false);  // Hide retry button when checking connection
        StartCoroutine(TestConnection(context));
    }

    private IEnumerator TestConnection(string context)
    {
        // Ensure the "Checking Connection..." message is displayed for at least 0.5 seconds
        yield return new WaitForSeconds(1.0f);

        using (UnityWebRequest www = UnityWebRequest.Head("http://70.119.225.15"))  // Use a lightweight endpoint
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                connectionStatusText.text = "Connection Failed: " + www.error;
                // Ensure the failure message is displayed for at least 0.5 seconds before showing retry button
                yield return new WaitForSeconds(1.0f);
                retryButton.gameObject.SetActive(true);  // Show retry button if connection fails
            }
            else
            {
                connectionStatusText.text = "Connection Successful";
                // Ensure the success message is displayed for at least 0.5 seconds before actions
                yield return new WaitForSeconds(1.0f);
                connectionPanel.SetActive(false);  // Hide the connection panel on success
                HandlePostConnectionSuccess(context);
            }
        }
    }

    private void HandlePostConnectionSuccess(string context)
    {
        switch (context)
        {
            case "initial":
            case "retry":
                ShowLogin();
                break;
            case "login":
                loginCanvas.GetComponent<LoginManager>().AttemptLogin();
                break;
            case "register":
                registrationCanvas.GetComponent<RegistrationManager>().AttemptRegister();
                break;
        }
    }

    public void ShowLogin()
    {
        loginCanvas.SetActive(true);
        registrationCanvas.SetActive(false);
    }

    public void ShowRegistration()
    {
        loginCanvas.SetActive(false);
        registrationCanvas.SetActive(true);
    }
}
