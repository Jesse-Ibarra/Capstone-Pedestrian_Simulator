using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWalker : MonoBehaviour
{
    public Transform katWalker;
    
    [Tooltip("Vertical offset above the capsule (Y axis)")]
    public float heightOffset = 1.36f;

    [Tooltip("Forward offset in front of the capsule")]
    public float forwardOffset = 0.2f;

    void LateUpdate()
    {
        if (katWalker != null)
        {
            // Calculate forward + upward offset based on capsule's rotation
            Vector3 offset = katWalker.up * heightOffset + katWalker.forward * forwardOffset;
            transform.position = katWalker.position + offset;
        }
    }
}


