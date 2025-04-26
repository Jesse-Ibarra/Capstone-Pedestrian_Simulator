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

    public Transform treadmillForward; // <- assign this in Inspector

    // Update is called once per frame
    void LateUpdate()
    {
        // Position the avatar body based on the head IK target + offset
        transform.position = head.ikTarget.position + headBodyPositionOffset;

        // Rotate based on treadmill's Y rotation, not headset
        float yaw = treadmillForward.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(transform.eulerAngles.x, yaw + headBodyYawOffset, transform.eulerAngles.z),
            turnSmoothness
        );

        // Apply IK mappings
        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
