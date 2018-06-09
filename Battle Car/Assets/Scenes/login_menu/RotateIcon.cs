
using UnityEngine;

public class RotateIcon : MonoBehaviour {

    [SerializeField]
    private float RotationSpeed;

    [SerializeField]
    private bool x;

    [SerializeField]
    private bool y;

    [SerializeField]
    private bool z;

    void Update()
    {
        if (x)
            this.transform.Rotate(Vector3.right * (RotationSpeed * Time.deltaTime));
        if (y)
            this.transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));
        if (z)
            this.transform.Rotate(Vector3.forward * (RotationSpeed * Time.deltaTime));
    }
}
