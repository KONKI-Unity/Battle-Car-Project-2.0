using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{

	public int code;
	private float disableTimer = 0;

	private void Update()
	{
		if (disableTimer > 0)
		{
			disableTimer -= Time.deltaTime;
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.name == "Sci-Fi_Soldier" && disableTimer<=0)
		{
			foreach (Teleport tp in FindObjectsOfType<Teleport>())
			{
				if (tp.code == code && tp != this)
				{
					tp.disableTimer = 2;
					Vector3 position = tp.gameObject.transform.position;
					position.y += 0.2f;
					collider.gameObject.transform.position = position;
				}
			}
		}
	}
}