using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference lookInput;
    
    [Space] [Header("Sway Settings")]
    [SerializeField] 
    private float speed;
    [SerializeField] 
    private float sensitivityMultiplier;

    private float yaw;
    private float pitch;
    private float yawChange;
    private float pitchChange;

    private void Update()
    {
        Quaternion rotationX = Quaternion.AngleAxis(-yawChange, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(pitchChange, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 look = context.ReadValue<Vector2>();
        look *= sensitivityMultiplier * Time.deltaTime;
        yawChange = look.x;
        pitchChange = look.y;
    }

    private void OnEnable()
    {
        lookInput.action.Enable();
        lookInput.action.performed += OnLook;
    }
    
    private void OnDisable()
    {
        lookInput.action.Disable();
        lookInput.action.performed -= OnLook;
    }
}