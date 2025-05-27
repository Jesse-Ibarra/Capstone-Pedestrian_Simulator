using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0, 1)]
    public float turnSmoothness = 0.1f;

    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;

    public Transform treadmillForward; // Assign in Inspector (the capsule transform)

    void LateUpdate()
    {
        if (head == null || treadmillForward == null)
            return;

        // --- 1. Position the avatar body based on the head IK target + offset
        transform.position = head.ikTarget.position + headBodyPositionOffset;

        // --- 2. Rotate avatar depending on treadmill vs debug
        float yaw;

        if (KATNativeSDK.GetWalkStatus().connected)
        {
            // Treadmill is connected → use treadmillForward Y rotation
            yaw = treadmillForward.eulerAngles.y;
        }
        else
        {
            // No treadmill → debug mode → use capsule (this transform's parent) rotation
            if (transform.parent != null)
                yaw = transform.parent.eulerAngles.y;
            else
                yaw = treadmillForward.eulerAngles.y; // fallback
        }

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(transform.eulerAngles.x, yaw + headBodyYawOffset, transform.eulerAngles.z),
            turnSmoothness
        );

        // --- 3. Map the IK targets
        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
