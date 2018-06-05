
using UnityEngine;
using UnityEngine.Networking;

public class MapSelect : MonoBehaviour {

    private NetworkManager networkManager;

    [SerializeField]
    private const int mapCount = 3;

    [SerializeField]
    private string[] sceneName = new string[mapCount] { "dev_map", "map2", "map3"};

    

    private int currentScene;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        currentScene = 0;
        ChangeMap(0);
    }

    

    public void ChangeMap(int value)
    {
        if (value >= mapCount || value < 0)
        {
            Debug.Log("Wrong index");
        }
        else
        {
            currentScene = value;
            networkManager.onlineScene = ("Scenes/" + sceneName[currentScene]);
            Debug.Log("Server has changed map to : " + sceneName[currentScene]);
        }
            
        
    }
}
