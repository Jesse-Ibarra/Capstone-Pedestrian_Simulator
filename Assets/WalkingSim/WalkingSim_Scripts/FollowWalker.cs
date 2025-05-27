using UnityEngine;

public class FollowWalker : MonoBehaviour
{
    public Transform katWalker;

    [Tooltip("Vertical offset above the capsule (Y axis)")]
    public float heightOffset = 0.6f;

    void LateUpdate()
    {
        if (katWalker != null)
        {
            
            // Follow capsule position + vertical offset
            Vector3 offset = Vector3.up * heightOffset;
            transform.position = katWalker.position + offset;
        }
    }
}
