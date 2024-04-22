using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RegistrationManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button registerButton;
    public Button backButton;
    public TextMeshProUGUI feedbackText;
    private string registerUrl = "http://70.119.225.15/RegisterUser.php";

    void Start()
    {
        registerButton.onClick.AddListener(AttemptRegister);
        backButton.onClick.AddListener(() => FindObjectOfType<ConnectionStatusManager>().ShowLogin());

        // Set up enter to submit form when editing is finished
        usernameInput.onSubmit.AddListener((value) => AttemptRegister());
        passwordInput.onSubmit.AddListener((value) => AttemptRegister());
        confirmPasswordInput.onSubmit.AddListener((value) => AttemptRegister());
    }

    public void AttemptRegister()
    {
        if (passwordInput.text != confirmPasswordInput.text)
        {
            feedbackText.text = "Passwords do not match.";
            return;
        }

        StartCoroutine(RegisterRoutine());
    }

    private IEnumerator RegisterRoutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", usernameInput.text);
        form.AddField("loginPass", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(registerUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Error connecting to server: " + www.error;
            }
            else
            {
                feedbackText.text = www.downloadHandler.text;  // Display server response as feedback
            }
        }
    }

    void Update()
    {
        // Check if Enter is pressed and the active object is one of the input fields
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (EventSystem.current.currentSelectedGameObject == usernameInput.gameObject ||
                EventSystem.current.currentSelectedGameObject == passwordInput.gameObject ||
                EventSystem.current.currentSelectedGameObject == confirmPasswordInput.gameObject)
            {
                AttemptRegister();
            }
        }
    }
}
