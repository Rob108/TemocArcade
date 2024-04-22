using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button regButton;
    public TextMeshProUGUI feedbackText;
    private string loginUrl = "http://70.119.225.15/Login.php";

    void Start()
    {
        loginButton.onClick.AddListener(AttemptLogin);
        regButton.onClick.AddListener(() => FindObjectOfType<ConnectionStatusManager>().ShowRegistration());

        // Set the initial focus to the username input
        usernameInput.Select();

        // Setup Enter and Tab navigation
        usernameInput.onEndEdit.AddListener(delegate { MoveToNext(usernameInput, passwordInput); });
        passwordInput.onEndEdit.AddListener(delegate { MoveToNext(passwordInput, loginButton.GetComponent<Selectable>()); });
    }

    public void AttemptLogin()
    {
        StartCoroutine(LoginRoutine());
    }

    private IEnumerator LoginRoutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", usernameInput.text);
        form.AddField("loginPass", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(loginUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Error connecting to server: " + www.error;
            }
            else
            {
                feedbackText.text = www.downloadHandler.text;
            }
        }
    }

    // This function moves focus based on pressing the Tab or Enter keys
    void MoveToNext(TMP_InputField currentField, Selectable next)
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            next.Select();
        }
    }

    void Update()
    {
        // Submit form when the Enter key is pressed and login button is focused
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && loginButton == EventSystem.current.currentSelectedGameObject.GetComponent<Button>())
        {
            AttemptLogin();
        }
    }
}

