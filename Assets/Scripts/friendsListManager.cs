using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class FriendsPanelManager : MonoBehaviour
{
    public TMP_InputField addFriendInput;
    public Button addButton;
    public Button backButton;
    public TMP_Text addFriendResponseText; // Text to display results from adding a friend
    public TMP_Text friendRequestUsernameText;  // Displays the most recent request or "No incoming requests."
    public Button acceptButton;
    public Button denyButton;
    public TMP_Text friendsListText;  // Displays friends list
    public GameObject friendsPanel;
    public GameObject dashboardUI;

    private string baseURL = "http://70.119.225.15/";
    private List<FriendRequestInfo> currentFriendRequests = new List<FriendRequestInfo>();  // To store incoming friend requests

    void Start()
    {
        addButton.onClick.AddListener(SendFriendRequest);
        acceptButton.onClick.AddListener(() => RespondToFriendRequest(true));
        denyButton.onClick.AddListener(() => RespondToFriendRequest(false));
        backButton.onClick.AddListener(() => Goback());
        LoadFriendsList();
        LoadFriendRequests();
    }

    private void SendFriendRequest()
    {
        StartCoroutine(ServerRequest(baseURL + "RequestFriend.php", new Dictionary<string, string>
        {
            {"playerID", PlayerPrefs.GetInt("UserID").ToString()},
            {"friendName", addFriendInput.text}
        }, response => {
            addFriendResponseText.text = response.Equals("0") ? "User not found or request failed." : "Friend request sent successfully!";
            Debug.Log("Add friend response: " + response);
        }));
    }

    private void RespondToFriendRequest(bool accept)
    {
        if (currentFriendRequests.Count > 0)
        {
            var request = currentFriendRequests[0];
            var endpoint = accept ? "AcceptFriendRequest.php" : "DeclineFriendRequest.php";
            StartCoroutine(ServerRequest(baseURL + endpoint, new Dictionary<string, string>
            {
                {"frienderID", request.player_id1},
                {"friendeeID", PlayerPrefs.GetInt("UserID").ToString()},
                {"requestDate", request.date}
            }, response => {
                Debug.Log("Friend request " + (accept ? "accepted" : "declined") + ": " + response);
                LoadFriendRequests();  // Refresh the list after a response
            }));
        }
    }

    private void LoadFriendsList()
    {
        StartCoroutine(ServerRequest(baseURL + "GetPlayerFriends.php", new Dictionary<string, string>
        {
            {"playerID", PlayerPrefs.GetInt("UserID").ToString()}
        }, HandleFriendsListResponse));
    }

    private void LoadFriendRequests()
    {
        StartCoroutine(ServerRequest(baseURL + "GetIncomingFriendRequests.php", new Dictionary<string, string>
        {
            {"playerID", PlayerPrefs.GetInt("UserID").ToString()}
        }, HandleFriendRequestsResponse));
    }

    IEnumerator ServerRequest(string url, Dictionary<string, string> formFields, System.Action<string> responseHandler)
    {
        WWWForm form = new WWWForm();
        foreach (var field in formFields)
        {
            form.AddField(field.Key, field.Value);
        }

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Server request failed: " + www.error);
                responseHandler("0");  // Handle error as "0" response
            }
            else
            {
                responseHandler(www.downloadHandler.text);
            }
        }
    }

    private void HandleFriendsListResponse(string response)
    {
        if (response == "0" || string.IsNullOrWhiteSpace(response))
        {
            friendsListText.text = "No new friends, go find some!";
        }
        else
        {
            var friends = JsonConvert.DeserializeObject<List<FriendInfo>>(response);
            friendsListText.text = friends != null && friends.Count > 0 ? string.Join("\n", friends.ConvertAll(f => f.username)) : "No new friends, go find some!";
        }
    }

    private void HandleFriendRequestsResponse(string response)
    {
        if (response == "0" || string.IsNullOrWhiteSpace(response))
        {
            friendRequestUsernameText.text = "No incoming requests.";
            acceptButton.gameObject.SetActive(false);
            denyButton.gameObject.SetActive(false);
        }
        else
        {
            currentFriendRequests = JsonConvert.DeserializeObject<List<FriendRequestInfo>>(response);
            if (currentFriendRequests.Count > 0)
            {
                friendRequestUsernameText.text = currentFriendRequests[0].username;
                acceptButton.gameObject.SetActive(true);
                denyButton.gameObject.SetActive(true);
            }
            else
            {
                friendRequestUsernameText.text = "No incoming requests.";
                acceptButton.gameObject.SetActive(false);
                denyButton.gameObject.SetActive(false);
            }
        }
    }

    public void Goback()
    {
        friendsPanel.SetActive(false);
        dashboardUI.SetActive(true);
    }
    public class FriendInfo
    {
        public string username;
        public int id;
    }

    public class FriendRequestInfo
    {
        public string username;
        public string player_id1;
        public string date;
    }
}

