using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFrameRate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 90; // Set to 90 FPS for smooth VR movement
        QualitySettings.vSyncCount = 0;  // Disable V-Sync to let targetFrameRate work

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
