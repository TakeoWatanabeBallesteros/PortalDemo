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
    private Transform pickableObjectPoint;
    [SerializeField] 
    private Transform cameraPosition;
    [SerializeField] 
    private float pickUpDistance;
    [SerializeField] 
    private LayerMask pickUpLayerMask;

    private PickableObject pickableObject;

    private void OnEnable()
    {
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        interactAction.action.performed += _ =>
        {
            if (pickableObject == null)
            {
                if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out RaycastHit raycastHit,
                        pickUpDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out pickableObject))
                    {
                        pickableObject.Pick(pickableObjectPoint);
                    }
                }
            }
            else
            {
                pickableObject.Drop();
                pickableObject = null;
            }
        };
    }
}
