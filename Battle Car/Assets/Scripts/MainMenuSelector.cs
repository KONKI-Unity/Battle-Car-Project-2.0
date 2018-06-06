using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSelector : MonoBehaviour {

    [SerializeField]
    private GameObject[] tabSet;

    private int currentTab = 0;

    private void Start()
    {
        currentTab = 0;
        SetTab(0);
    }

    private void Reset()
    {
        for(int i = 0; i < tabSet.Length; i++)
        {
            tabSet[i].SetActive(false);
        }
    }

    public void SetTab(int index)
    {
        currentTab = index;
        Reset();
        tabSet[index].SetActive(true);
        Debug.Log(tabSet[index].name + " has been loaded");

    }

    public void Exit()
    {
        Application.Quit();
    }

}
