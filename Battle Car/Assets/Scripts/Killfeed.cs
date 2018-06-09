using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killfeed : MonoBehaviour {

    [SerializeField]
    GameObject killfeedItemPrefab;



	void Start () {
        GameManager.instance.onPlayerKilledCallback += OnKill;
	}
	
	public void OnKill (string player, string source, string action)
    {
        GameObject go = (GameObject)Instantiate(killfeedItemPrefab, this.transform);
        go.GetComponent<KillfeedItem>().Setup(player, source, action);

        Destroy(go, 4f);
    }
}
