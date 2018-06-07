
using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour
{

    public Text usernameText;

    [SerializeField]
    public Text playerStat;

    void Start()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            usernameText.text = UserAccountManager.PlayerUsername;
            playerStat.text = "Player " + UserAccountManager.PlayerUsername;
        }
    }



    public void LogOut()
    {
        if (UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.LogOut();
    }

}
