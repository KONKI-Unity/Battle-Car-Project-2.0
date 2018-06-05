using UnityEngine;
using UnityEngine.Networking;

public class HostGame : NetworkBehaviour {

    [SerializeField]
    private uint roomSize = 6;


    private string roomName = "";

    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
        Debug.Log("Room name has been set to " + _name+ " !");
    }

    public void SetRoomSize(string size)
    {
        uint.TryParse(size, out roomSize);
    }

    public void CreateRoom()
    {
        Debug.Log("Click");
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating room " + roomName + " with " + roomSize + " slots");
            //
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }

}
