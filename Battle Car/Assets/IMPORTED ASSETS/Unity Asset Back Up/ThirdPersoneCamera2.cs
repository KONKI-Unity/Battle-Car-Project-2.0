using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersoneCamera2 : MonoBehaviour {

    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float damping;

    public Transform cameraLookTarget;
    public GameObject localPlayer;
    private Camera cam;

    private void Start()
    {

        cameraLookTarget = localPlayer.transform.Find("cameraLookTarget");
        if (cameraLookTarget == null)
            cameraLookTarget = localPlayer.transform;
    }

    private void Update()
    {
        Vector2 direction = new Vector2();
        Vector3 targetPosition = cameraLookTarget.position + localPlayer.transform.forward * cameraOffset.z + localPlayer.transform.up * cameraOffset.y + localPlayer.transform.right * cameraOffset.x;
        Quaternion targetRotation = Quaternion.LookRotation(cameraLookTarget.position - targetPosition, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, damping * Time.deltaTime);
    }
}
