using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChooseCar : MonoBehaviour {

    private NetworkManager networkManager;


    [SerializeField]
    private GameObject[] carSet;

    private int currentCar;

    public GameObject GetCurrentCar()
    {
        return carSet[currentCar];
    }

    private void Start()
    {
        currentCar = 1;

        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null && carSet.Length != 0)
        {
            networkManager.playerPrefab = carSet[0];
        }

    }

    private void Reset()
    {
        networkManager.playerPrefab = null;
    }

    public void SetCar(int index)
    {
        if(index >= 0 && index < carSet.Length)
        {
            currentCar = index;
            Reset();
            networkManager.playerPrefab = carSet[index];
            Debug.Log("Car selected is : " + carSet[index].name);
        }
    }


}

