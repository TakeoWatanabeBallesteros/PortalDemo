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

    public PickableObject pickableObject;
    private PortalableObject portable;

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
    }

    private void Pick(InputAction.CallbackContext context)
    {
        if (pickableObject == null)
        {
            if (!Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit,
                    pickUpDistance, pickUpLayerMask)) return;
            if (!raycastHit.transform.TryGetComponent(out pickableObject)) return;
            pickableObject.Pick(pickableObjectPoint);
            if (raycastHit.transform.TryGetComponent(out portable)) portable.onHold = true;
            Physics.IgnoreCollision(playerCollider, pickableObject._collider, true);
            throwAction.action.performed += Throw;
            PlayerFSM.ChangeShoot();
        }
        else
        {
            pickableObject.Drop();
            Physics.IgnoreCollision(playerCollider, pickableObject._collider, false);
            pickableObject = null;
            if(portable != null) portable.onHold = false;
            portable = null;
            throwAction.action.performed -= Throw;
            PlayerFSM.ChangeShoot();
        }
    }

    private void Throw(InputAction.CallbackContext context)
    {
        pickableObject.rigidBody.AddForce(cameraPosition.forward * 1000.0f);
        pickableObject.Drop();
        Physics.IgnoreCollision(playerCollider, pickableObject._collider, false);
        pickableObject = null;
        if(portable != null) portable.onHold = false;
        portable = null;
        throwAction.action.performed -= Throw;
        PlayerFSM.ChangeShoot();
    }
}