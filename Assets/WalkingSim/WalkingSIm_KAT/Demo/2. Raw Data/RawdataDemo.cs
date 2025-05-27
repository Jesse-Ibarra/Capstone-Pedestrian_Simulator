using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawdataDemo : MonoBehaviour
{
    void FixedUpdate()
    {
        var data = KATNativeSDK.GetWalkStatus();
        var str = "Device:" + data.deviceName + "\n";
        str += "    Connected:" + data.connected + "\n";
        str += "    Body Rotation:" + data.bodyRotationRaw.ToString("f4") + "\n";
        str += "    Speed:" + data.moveSpeed.ToString("f4") + "\n";

        var text = GetComponent<Text>();
        text.text = str;
    }
}
