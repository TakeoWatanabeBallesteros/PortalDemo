using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpDrop : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference interactAction;
    [SerializeField] 
    private InputActionReference throwAction;
    [SerializeField] 
    private Transform pickableObjectPoint;
    [SerializeField] 
    private Transform cameraPosition;
    [SerializeField] 
    private float pickUpDistance;
    [SerializeField] 
    private LayerMask pickUpLayerMask;
    [SerializeField] 
    private Collider playerCollider;

    private PickableObject pickableObject;

    private void OnEnable()
    {
        interactAction.action.Enable();
        throwAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
        throwAction.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        interactAction.action.performed += Pick;
        throwAction.action.performed += Throw;
    }

    private void Pick(InputAction.CallbackContext context)
    {
        if (pickableObject == null)
        {
            if (!Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit,
                    pickUpDistance, pickUpLayerMask)) return;
            if (!raycastHit.transform.TryGetComponent(out pickableObject)) return;
            pickableObject.Pick(pickableObjectPoint);
            Physics.IgnoreCollision(playerCollider, pickableObject._collider, true);
        }
        else
        {
            pickableObject.Drop();
            Physics.IgnoreCollision(playerCollider, pickableObject._collider, false);
            pickableObject = null;
        }
    }

    private void Throw(InputAction.CallbackContext context)
    {
        if(pickableObject == null) return;
        var direction = (pickableObjectPoint.forward * 10) - pickableObject.transform.position;
        pickableObject.rigidBody.AddForce(direction * 100.0f);
        pickableObject.Drop();
        Physics.IgnoreCollision(playerCollider, pickableObject._collider, false);
        pickableObject = null;
    }
}
