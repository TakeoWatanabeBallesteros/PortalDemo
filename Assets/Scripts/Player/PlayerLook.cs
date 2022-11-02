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
    private Transform playerHead;
    [SerializeField] 
    private float sensibility;
    [SerializeField] 
    private float bottomClamp;
    [SerializeField] 
    private float topClamp;

    private float yaw;
    private float pitch;
    
    private void OnEnable()
    {
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
        pitch = ClampAngle(pitch, bottomClamp, topClamp);
        playerHead.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}