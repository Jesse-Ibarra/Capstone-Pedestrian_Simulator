using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WalkDemo : MonoBehaviour
{
    public Text infoText;

    void FixedUpdate()
    {
        var data = KATNativeSDK.GetWalkStatus();
        var str = "Device:" + data.deviceName + "\n";
        str += "    Connected:" + data.connected + "\n";
        str += "    Body Rotation:" + data.bodyRotationRaw.ToString("f4") + "\n";
        str += "    Speed:" + data.moveSpeed.ToString("f4") + "\n";

        infoText.text = str;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}
