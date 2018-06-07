
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;
using System.Collections;

public class UserAccountManager : MonoBehaviour {

    public static UserAccountManager instance;

    public static string PlayerUsername { get; protected set; }
    private static string PlayerPassword = "";

    public static bool IsLoggedIn { get; protected set; }

    public string loggedInSceneName = "main_menu2";
    public string loggedOutSceneName = "Login";

    public delegate void OnDataReceivedCallback(string data);


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }


    public void LogOut()
    {
        PlayerUsername = "";
        PlayerPassword = "";

        IsLoggedIn = false;

        Debug.Log("User logged out!");

        SceneManager.LoadScene(loggedOutSceneName);
    }

    public void LogIn(string username, string password)
    {
        PlayerUsername = username;
        PlayerPassword = password;

        IsLoggedIn = true;

        Debug.Log("Logged in as " + username);

        SceneManager.LoadScene(loggedInSceneName);
    }


  

    public void GetData(OnDataReceivedCallback onDataReceived)
    { //called when the 'Get Data' button on the data part is pressed

        if (IsLoggedIn)
        {
            StartCoroutine(requestGetData(PlayerUsername, PlayerPassword, onDataReceived)); //calls function to send get data request
        }
    }



    IEnumerator requestGetData(string username, string password, OnDataReceivedCallback onDataReceived)
    {
        string data = "ERROR";

        IEnumerator e = DCF.GetUserData(username, password); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
        }
        else
        {
            if (response == "ContainsUnsupportedSymbol")
            {
                //One of the parameters contained a - symbol
                Debug.Log("Get Data Error: Contains Unsupported Symbol '-'");
            }
            else
            {
                //Data received in returned.text variable
                string DataRecieved = response;
                data = DataRecieved;
            }
        }

        if (onDataReceived != null)
            onDataReceived.Invoke(data);
    }

    public void SetData(string data)
    {
        if (IsLoggedIn)
        {
            StartCoroutine(requestSetData(PlayerUsername, PlayerPassword, data));
        }
    }

    IEnumerator requestSetData(string username, string password, string data)
    {
        IEnumerator e = DCF.SetUserData(username, password, data); ; // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            Debug.Log("Data sent succefully");
        }
        else
        {
            Debug.Log("UserAccManager SetData : Some error has been made ");
        }
    }

}
