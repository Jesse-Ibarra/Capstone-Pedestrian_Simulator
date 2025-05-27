using UnityEngine;

public class Position : MonoBehaviour
{
    public Transform xrRig;      // Reference to the XRRig
    public Transform katWalker;  // Reference to the KATDemoWalker

    void Start()
    {
        if (xrRig == null || katWalker == null)
        {
            Debug.LogWarning("XRRig or KATDemoWalker not assigned.");
            return;
        }

        foreach (Transform child in xrRig)
        {
            Vector3 newPos = child.position;
            newPos.x = katWalker.position.x;
            newPos.z = katWalker.position.z;
            child.position = newPos;
        }

        Debug.Log(" XRRig children aligned to KATDemoWalker at start.");
    }
}
