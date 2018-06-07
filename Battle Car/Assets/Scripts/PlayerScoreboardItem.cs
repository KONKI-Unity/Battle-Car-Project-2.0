﻿
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour
{

    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text killsText;

    [SerializeField]
    Text deathsText;

    public void Setup(string username, int kills, int deaths)
    {
        usernameText.text = username;
        killsText.text = kills.ToString();
        deathsText.text = deaths.ToString();
    }

}