using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.LowLevel;

public class PlayerLook : MonoBehaviour, IReset
{
    [SerializeField] 
    private InputActionReference lookInput;
    [SerializeField] 
    private InputActionReference zoomInput;
    [SerializeField] 
    private InputActionReference mouseLock;
    [SerializeField] 
    private Transform playerHead;
    [SerializeField] 
    private Camera camera1;
    [SerializeField] 
    private Camera camera2;
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
    private float zoom = 60.0f;
    
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
        zoomInput.action.Enable();
    }

    private void OnDisable()
    {
        lookInput.action.Disable();
        zoomInput.action.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        sensibility= GameManager.GetGameManager().sensibility;
        yaw = 0;
        pitch = 0;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;

        zoomInput.action.performed += Zoom;
    }

    // Update is called once per frame
    private void Update()
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

    private void Zoom(InputAction.CallbackContext context)
    {
        if(context.ReadValue<Vector2>().y == 0) return;
        if(context.ReadValue<Vector2>().y > 0)
        {
            zoom = zoom switch
            {
                60f => 45f,
                45f => 35f,
                _ => zoom
            };
        }
        else if(context.ReadValue<Vector2>().y < 0)
        {
            zoom = zoom switch
            {
                45f => 60f,
                35f => 45f,
                _ => zoom
            };
        }
        camera1.fieldOfView = zoom;
        camera2.fieldOfView = zoom + 20;
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
    }

    public float GetPitch()
    {
        return pitch;
    }

    public void Reset()
    {
        yaw = transform.rotation.y;
        pitch = playerHead.localRotation.x;
        zoom = 60;
        camera1.fieldOfView = zoom;
        camera2.fieldOfView = zoom + 20;
    }
}
