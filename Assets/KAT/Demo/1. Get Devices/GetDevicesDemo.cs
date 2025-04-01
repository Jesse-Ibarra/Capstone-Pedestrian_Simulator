using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GetDevicesDemo : MonoBehaviour
{
    void Update()
    {

        var count = KATNativeSDK.DeviceCount();
        var str = "Found " + count + " Devices:\n";

        for(var i = 0u; i < count;i++)
        {
            var gdd = KATNativeSDK.GetDevicesDesc(i);
            str += " DeviceName:" + gdd.device + " SN:" + gdd.serialNumber + " \n";
        }

        var text = GetComponent<Text>();
        text.text = str;
    }
}
