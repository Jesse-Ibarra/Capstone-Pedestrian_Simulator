using UnityEngine;

public class HandTargetOffset : MonoBehaviour
{
    public Transform controllerTransform; // Assign the XR controller here
    public Vector3 positionOffset = new Vector3(0f, -0.1f, 0.15f); // Tune this
    public Vector3 rotationOffsetEuler = new Vector3(0f, 90f, 0f); // Optional

    void LateUpdate()
    {
        transform.position = controllerTransform.position + controllerTransform.rotation * positionOffset;
        transform.rotation = controllerTransform.rotation * Quaternion.Euler(rotationOffsetEuler);
    }
}
