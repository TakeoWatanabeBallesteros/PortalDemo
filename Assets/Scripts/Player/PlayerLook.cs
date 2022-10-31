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
    private InputActionReference lookAction;
    [SerializeField] 
    private Transform playerHead;
    [SerializeField] 
    private float sensibility;

    private float yaw;
    private float pitch;
    
    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
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
        Vector2 input = lookAction.action.ReadValue<Vector2>();
        yaw += input.x * sensibility * Time.deltaTime;
        pitch -= input.y * sensibility * Time.deltaTime;
        playerHead.localRotation = Quaternion.Euler(pitch, yaw, 0.0f);
    }
}
