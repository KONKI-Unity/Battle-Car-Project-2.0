
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{

    public Text killCount;
    public Text deathCount;

    // Use this for initialization
    void Start()
    {
        if (UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.GetData(OnReceivedData);
    }

    void OnReceivedData(string data)
    {
        if (killCount == null || deathCount == null)
            return;

        Debug.Log("data received : " + data);
        killCount.text = " Kills count : " + DataTranslator.DataToKills(data).ToString();
        deathCount.text = " Deaths count : " + DataTranslator.DataToDeaths(data).ToString();
    }

}