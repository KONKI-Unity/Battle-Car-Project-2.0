using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class myip : MonoBehaviour {

	public Text myIP;
	// Use this for initialization
	void Start () {

		myIP.text = "My ip: " + Network.player.ipAddress;
	}

}
