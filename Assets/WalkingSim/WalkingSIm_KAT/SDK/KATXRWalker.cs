using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using static KATXRWalker;

public class KATXRWalker : MonoBehaviour
{
    public GameObject xr;
    public GameObject eye;
    public GameObject cameraOffset;

    public enum ExecuteMethod
    {
        RigidBody,
        CharactorController,
        MovePosition
    }
    public ExecuteMethod executeMethod = ExecuteMethod.CharactorController;

    [Header("SpeedSettings")]
    [Range(0.5f, 10.0f)]
    public float speedMul = 1.0f;
    public enum SpeedMode
    {
        [Tooltip("Movement speed increases as actual speed increases")]
        linear,
        [Tooltip("Movement speed remains constant")]
        constant
    }
    [Tooltip("Character movement speed mode")]
    public SpeedMode speedMode = SpeedMode.linear;
    [Range(0.0f, 6.0f)]
    [Tooltip("Movement speed in constant mode")]
    public float constantSpeed = 2.0f;
    [Range(0.0f, 6.0f)]
    [Tooltip("Only when the actual speed exceeds this value will it move")]
    public float constantSpeedThreshold = 1.0f;

    protected Vector3 lastPosition = Vector3.zero;
    protected float yawCorrection;
    [Header("Debug Mode (No Treadmill)")]
    public bool debugMode = true;
    public float yawTurnSpeed = 45f; // degrees per second
    private float simulatedYaw = 0f;
    private Quaternion simulatedBodyRotation = Quaternion.identity;

    void Start()
    {

    }

    void FixedUpdate()
    {
        var ws = KATNativeSDK.GetWalkStatus();
        bool treadmillConnected = ws.connected;

        // Fake walkStatus for debugging if treadmill not connected
        if (!treadmillConnected)
        {
            ws.connected = false;
            ws.moveSpeed = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) ws.moveSpeed.z = 1;
            if (Input.GetKey(KeyCode.S)) ws.moveSpeed.z = -1;
            if (Input.GetKey(KeyCode.D)) ws.moveSpeed.x = 1;
            if (Input.GetKey(KeyCode.A)) ws.moveSpeed.x = -1;

            float stickX = Input.GetAxis("Horizontal");
            float stickY = Input.GetAxis("Vertical");
            ws.moveSpeed.x += stickX;
            ws.moveSpeed.z += stickY;

            float rightStickX = Input.GetAxis("RightStickHorizontal");
            simulatedYaw += rightStickX * yawTurnSpeed * Time.fixedDeltaTime;

            if (Input.GetKey(KeyCode.Q)) simulatedYaw -= yawTurnSpeed * Time.fixedDeltaTime;
            if (Input.GetKey(KeyCode.E)) simulatedYaw += yawTurnSpeed * Time.fixedDeltaTime;

            simulatedBodyRotation = Quaternion.Euler(0, simulatedYaw, 0);
            ws.bodyRotationRaw = simulatedBodyRotation;

            if (cameraOffset != null)
                cameraOffset.transform.rotation = simulatedBodyRotation;

            if (eye != null)
                eye.transform.rotation = simulatedBodyRotation;
        }


        // Calibration
        var lastCalibrationTime = KATNativeSDK.GetLastCalibratedTimeEscaped();

        if (ws.deviceDatas[0].btnPressed || lastCalibrationTime < 0.08)
        {
            var hmdYaw = eye.transform.eulerAngles.y;
            var bodyYaw = ws.bodyRotationRaw.eulerAngles.y;
            float yawDelta = bodyYaw - hmdYaw;

            var pos = transform.position;
            var eyePos = eye.transform.position;
            pos.x = eyePos.x;
            pos.z = eyePos.z;
            pos.y = eyePos.y; 
            transform.position = pos;

            lastPosition = transform.position;
            return;
        }

        // Apply rotation from treadmill (chest direction only)
        Vector3 euler = ws.bodyRotationRaw.eulerAngles;
        euler.y *= 1f; // Multiply Yaw
        transform.rotation = Quaternion.Euler(euler);


        // Speed mode logic
        switch (speedMode)
        {
            case SpeedMode.linear:
                break;
            case SpeedMode.constant:
                if (ws.moveSpeed.magnitude >= constantSpeedThreshold)
                    ws.moveSpeed = Vector3.Lerp(ws.moveSpeed, ws.moveSpeed.normalized * constantSpeed, 0.2f);
                else
                    ws.moveSpeed = Vector3.zero;
                break;
        }

        // Final movement direction: separate logic for debug vs treadmill
        Vector3 bodyForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 bodyRight = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        Vector3 moveDir = treadmillConnected
            ? bodyForward * ws.moveSpeed.z * speedMul
            : (bodyForward * ws.moveSpeed.z + bodyRight * ws.moveSpeed.x) * speedMul;


        // Execute movement
        switch (executeMethod)
        {
            case ExecuteMethod.CharactorController:
                GetComponent<CharacterController>().SimpleMove(moveDir);
                break;
            case ExecuteMethod.MovePosition:
                transform.position += moveDir * Time.fixedDeltaTime;
                break;
            case ExecuteMethod.RigidBody:
                GetComponent<Rigidbody>().velocity = moveDir;
                break;
        }
    }

    void LateUpdate()
{

    // Keep XR rig smoothly following the capsule’s position
    var smoothOffset = Vector3.Lerp(lastPosition, transform.position, 0.15f);
    xr.transform.position += smoothOffset - lastPosition;
    lastPosition = transform.position;
}

}
