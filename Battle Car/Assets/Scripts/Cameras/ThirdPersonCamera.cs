using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThirdPersonCamera : MonoBehaviour {

    private const float Y_ANGLE_MIN = -60f;
    private const float Y_ANGLE_MAX = 60.0f;

    public Transform lookAt;
    public Transform camTransform;
    public bool visible = false;
    private Camera cam;
    public float offsetX = 0.0f;
    public float offsetY = 0.0f;
    public float distance = 5.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float sensivityX = 4.0f;
    private float sensivityY = 1.0f;

    private void Start()
    {
        camTransform = transform;
        cam = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            visible = !visible;

            if (visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        currentX += Input.GetAxis("Mouse X");
        currentY -= Input.GetAxis("Mouse Y");
        distance -= Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance, 2f, 8);
        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {
        Vector3 direction = new Vector3(offsetX, offsetY, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        camTransform.position = lookAt.position + rotation * direction;
        Vector3 tmp = lookAt.position;
        tmp.y += offsetY;
        tmp.x += offsetX;
        camTransform.LookAt(tmp);
    }
}
