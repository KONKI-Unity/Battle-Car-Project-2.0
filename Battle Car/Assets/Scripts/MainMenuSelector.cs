using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSelector : MonoBehaviour {

    [SerializeField]
    private GameObject[] buttonSet;

    private int currentTab = 0;

    private void Start()
    {
        currentTab = 0;
        SetTab(0);
    }

    private void Reset()
    {
        for(int i = 0; i < buttonSet.Length; i++)
        {
            buttonSet[i].active = false;
        }
    }

    public void SetTab(int index)
    {
        currentTab = index;
        Reset();
        buttonSet[index].active = true;
        Debug.Log(buttonSet[index].name + " has been loaded");

    }

    public void Exit()
    {
        Application.Quit();
    }

}
