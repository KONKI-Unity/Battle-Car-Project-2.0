
using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour {

    [SerializeField]
    Text player1;

    [SerializeField]
    Text action;

    [SerializeField]
    Text player2;

    public void Setup(string player, string source, string _action)
    {
        player1.text = source;
        action.text = _action;
        player2.text = player;


    }

}
