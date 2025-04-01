using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script is responsible for wrapping VR input inside easy-to-use functions such as `ForwardPressed()`
/// All future VR integration should be done through this script. 
/// Simply add the appropriate SDK or API to this script, and inside the function you need, make the cooresponding function call to the library that you are using
/// </summary>


// This script is actually doing the same thing that OpenXR is meant to do. 
// It is still useable, but consider exporting everything out of this script into the XR Interaction Toolkit settings 
// This can be found under Assets/Samples/XR Interaction Toolkit/1.0.0-prev.3/Default Input Actions/XRI Default Input Actions.inputactions

public class VRInput : MonoBehaviour
{
    public GameObject viveRightControl;
    public GameObject viveLeftControl;
    private XRController viveRightHand;
    private XRController viveLeftHand;
    //private ActionBasedController viveLeftHand;

    //private void Update()
    //{
    //    float triggerValue;
    //    /*if (viveRightHand.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
    //    {
    //        Debug.Log("Trigger button is pressed.");
    //    }*/

    //    ///
    //    var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
    //    UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);

    //    if (leftHandDevices.Count == 1)
    //    {
    //        UnityEngine.XR.InputDevice device = leftHandDevices[0];
    //        //Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.characteristics.ToString()));
    //        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out triggerValue))
    //        {
    //            Debug.Log("Trigger button is pressed.");
    //        }
    //        /*var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();
    //        if (device.TryGetFeatureUsages(inputFeatures))
    //        {
    //            foreach (var feature in inputFeatures)
    //            {
    //                if (feature.type == typeof(bool))
    //                {
    //                    bool featureValue;
    //                    if (device.TryGetFeatureValue(feature.As<bool>(), out featureValue))
    //                    {
    //                        Debug.Log(string.Format("Bool feature {0}'s value is {1}", feature.name, featureValue.ToString()));
    //                    }
    //                }
    //            }
    //        }*/
    //    }
    //    else if (leftHandDevices.Count > 1)
    //    {
    //        Debug.Log("Found more than one left hand!");
    //    }
    //    /// right hand 
    //    var RightHandDevices = new List<UnityEngine.XR.InputDevice>();
    //    UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, RightHandDevices);

    //    if (RightHandDevices.Count == 1)
    //    {
    //        UnityEngine.XR.InputDevice device = RightHandDevices[0];
    //        //Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.characteristics.ToString()));
    //        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out triggerValue))
    //        {
    //            Debug.Log("Trigger button is pressed.");
    //        }
    //        /*var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();
    //        if (device.TryGetFeatureUsages(inputFeatures))
    //        {
    //            foreach (var feature in inputFeatures)
    //            {
    //                if (feature.type == typeof(bool))
    //                {
    //                    bool featureValue;
    //                    if (device.TryGetFeatureValue(feature.As<bool>(), out featureValue))
    //                    {
    //                        Debug.Log(string.Format("Bool feature {0}'s value is {1}", feature.name, featureValue.ToString()));
    //                    }
    //                }
    //            }
    //        }*/
    //    }
    //    else if (RightHandDevices.Count > 1)
    //    {
    //        Debug.Log("Found more than one left hand!");
    //    }
    //}
    public void Start()
    {
        viveRightHand = viveRightControl.gameObject.GetComponent<XRController>();
        viveLeftHand = viveLeftControl.gameObject.GetComponent<XRController>();
        Debug.Log(viveLeftHand.ToString() + viveRightHand.ToString());
        Debug.Log(UnityEngine.Input.GetJoystickNames());
    }
    /*
    public bool ForwardPressed()
    {
        bool triggerPress;
        //Debug.Log("Checking forward input");
        if (GetOculusYPressed())     
        {
            return true;
        }
        else if (GetOculusRightIndexTrigger() != 0)
        {
            return true;
        }
        //else if (viveRightHand.activateAction.action.ReadValue<float>() != 0)
        //{
        //    Debug.Log("vive right");
        //    return true;
        //}
        else if ((viveRightHand.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerPress) && triggerPress) || UnityEngine.InputSystem.Gamepad.current.rightTrigger.isPressed)
        {
            //Debug.Log("vive right");
            return true;
        }
        else
        {
            return false;
        }

    }
    */
    /*
    public bool BackPressed()
    {
        bool triggerPress1;
        //Debug.Log("Checking back input");
        if (GetOculusXPressed())
        {
            return true;
        }
        else if (GetOculusLeftIndexTrigger() != 0)
        {
            return true;
        }
      
        else if ((viveLeftHand.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerPress1) && triggerPress1) || UnityEngine.InputSystem.Gamepad.current.leftTrigger.isPressed)
        {
            Debug.Log("vive left");
            return true;
        }
        else
        {
            return false;
        }
    }*/
    public bool RightPressed()
    {
        bool triggerPress1;
        //Debug.Log("Checking back input");
        //if (GetOculusXPressed())
        //{
        //    return true;
        //}
        //else if (GetOculusLeftIndexTrigger() != 0)
        //{
        //    return true;
        //}
        /*else if (viveLeftHand.activateAction.action.ReadValue<float>() != 0)
        {
            Debug.Log("vive left");
            return true;
        }*/
        if ((viveRightHand.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerPress1) && triggerPress1) || UnityEngine.InputSystem.Gamepad.current.dpad.right.isPressed)
        {
            //Debug.Log("vive turn right");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool LeftPressed()
    {
        bool triggerPress1;
        //Debug.Log("Checking back input");
        //if (GetOculusXPressed())
        //{
        //    return true;
        //}
        //else if (GetOculusLeftIndexTrigger() != 0)
        //{
        //    return true;
        //}
        /*else if (viveLeftHand.activateAction.action.ReadValue<float>() != 0)
        {
            Debug.Log("vive left");
            return true;
        }*/
        if ((viveLeftHand.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerPress1) && triggerPress1) || UnityEngine.InputSystem.Gamepad.current.dpad.left.isPressed)
        {
            //Debug.Log("vive turn left");
            return true;
        }
        else
        {
            return false;
        }
    }
    /*
    public float OculusLeftControllerHorizontal()
    {
        return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)[0];  // returns left-right value of the left-hand controller's joystick
    }
    public float GetOculusLeftIndexTrigger()
    {
        return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);   // left index trigger
    }

    public float GetOculusRightIndexTrigger()
    {
        //Debug.Log("Oculus right trigger: " + OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));
        return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);   // right index trigger
    }
    public float GetOculusLeftHandTrigger()
    {
        return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);  // left hand trigger
    }
    public float GetOculusRightHandTrigger()
    {
        return OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);  // right hand trigger
    }
    public bool GetOculusYPressed()
    {
        return OVRInput.Get(OVRInput.Button.Four) && (OVRInput.Get(OVRInput.RawButton.Y));  // Oculus 'Y' button
    }
    public bool GetOculusXPressed()
    {
        return OVRInput.Get(OVRInput.Button.Three) && (OVRInput.Get(OVRInput.RawButton.X)); // Oculus 'X' button
    }



    public void detectInput()
    {
        if (OVRInput.Get(OVRInput.Button.One) && (OVRInput.Get(OVRInput.RawButton.A)))
        {
            Debug.Log("OVRinput.Button.One: A");
        }

        if (OVRInput.Get(OVRInput.Button.Two) && (OVRInput.Get(OVRInput.RawButton.B)))
        {
            Debug.Log("OVRinput.Button.Two: B");
        }
        if (OVRInput.Get(OVRInput.Button.Three) && (OVRInput.Get(OVRInput.RawButton.X)))
        {
            Debug.Log("OVRinput.Button.Three: X");
        }
        if (OVRInput.Get(OVRInput.Button.Four) && (OVRInput.Get(OVRInput.RawButton.Y)))
        {
            Debug.Log("OVRinput.Button.Four: Y");
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0)
        {
            Debug.Log("Left index trigger");
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) != 0)
        {
            Debug.Log("Left hand trigger");
        }
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) != 0)
        {
            Debug.Log("Right index trigger");
        }
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) != 0)
        {
            Debug.Log("Right hand trigger");
        }

        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != new Vector2(0, 0))
        {
            Debug.Log("Left thumbstick moving: " + (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)));
        }
        if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != new Vector2(0, 0))
        {
            Debug.Log("Right thumbstick moving");
        }
    }
    */
}
