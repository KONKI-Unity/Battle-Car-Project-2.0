using UnityEngine;
using System.Collections;

// GUIDEdrive is the lite version of the GUIDE script
// used to control the cars in the ARK project.
// ARK = Arcade Racing Kit by New Creations!

[RequireComponent(typeof(GOdrive))]

public class GUIDEdrive : MonoBehaviour {

		private GOdrive vechileGUIDE;

		
		void Start () 
		{
			vechileGUIDE = GetComponent<GOdrive>();
		}
		
		void Update () 
		{
				vechileGUIDE.Thrust = Input.GetAxis("Vertical");
				vechileGUIDE.Steering = Input.GetAxis("Horizontal");						
		}
	}
	