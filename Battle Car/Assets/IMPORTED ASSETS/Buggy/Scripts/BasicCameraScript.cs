using UnityEngine;
using System.Collections;

public class BasicCameraScript : MonoBehaviour {

	[SerializeField] private Transform target;

	void Start () { }
	

	void LateUpdate () {

		if (!target) return;

		var wantedRotationAngle = target.eulerAngles.y;
		var wantedHeight = target.position.y + 2.4f;
		var currentRotationAngle = transform.eulerAngles.y;
		var currentHeight = transform.position.y;
		
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, 4f * Time.deltaTime);

		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, 4f * Time.deltaTime);

		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * 5f;
		transform.position = new Vector3(transform.position.x ,currentHeight , transform.position.z);

		transform.LookAt(target);
	}
}
