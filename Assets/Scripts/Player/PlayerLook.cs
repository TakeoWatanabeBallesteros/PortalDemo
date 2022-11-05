using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.LowLevel;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference lookInput;
    [SerializeField] 
    private InputActionReference mouseLock;
    [SerializeField] 
    private Transform playerHead;
    [SerializeField] 
    private float sensibility;
    [SerializeField] 
    private float bottomClamp;
    [SerializeField] 
    private float topClamp;

    private float yaw;
    private float pitch;
    private bool cameraLocked;
    private Quaternion direction;
    
    private void OnEnable()
    {
#if UNITY_EDITOR
        mouseLock.action.Enable();
        mouseLock.action.performed += _ =>
        {
            cameraLocked = !cameraLocked;
            if (cameraLocked)
            {
                lookInput.action.Disable();
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                lookInput.action.Enable();
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = cameraLocked;
        };
#endif
        lookInput.action.Enable();
    }

    private void OnDisable()
    {
        lookInput.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        yaw = 0;
        pitch = 0;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    private void Look()
    {
        Vector2 input = lookInput.action.ReadValue<Vector2>();
        yaw += input.x * sensibility * Time.deltaTime;
        pitch -= input.y * sensibility * Time.deltaTime;
        if(pitch > 180.0f)
        {
            pitch -= 360.0f;
        }
        pitch = ClampAngle(pitch, bottomClamp, topClamp);
        playerHead.localRotation = Quaternion.Slerp(playerHead.localRotation, Quaternion.Euler(pitch, 0.0f, 0.0f), Time.deltaTime * 15.0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, yaw, 0.0f), Time.deltaTime * 15.0f);
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


    public void ResetTargetRotation()
    {
        direction = Quaternion.LookRotation(playerHead.forward, Vector3.up);
        yaw = direction.eulerAngles.y;
        pitch = direction.eulerAngles.x;
        // pitch = ClampAngle(pitch, bottomClamp, topClamp);
        // playerHead.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
        // transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
    }
}
