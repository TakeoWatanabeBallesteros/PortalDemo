using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PortalBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Camera camera;
    [SerializeField] 
    private Transform otherPortalTransform;
    [SerializeField] 
    private PortalBehaviour mirrorPortal;
    [SerializeField] 
    private Transform playerPosition;
    
    
    private void Start()
    {
        
    }

    
    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 playerWorldPosition = playerPosition.position;
        Vector3 playerLocalPosition = otherPortalTransform.InverseTransformPoint(playerWorldPosition);
        mirrorPortal.camera.transform.position = mirrorPortal.transform.TransformPoint(playerLocalPosition);

        Vector3 playerWorldDirection = Camera.main.transform.forward;
        Vector3 playerLocalDirection = otherPortalTransform.InverseTransformDirection(playerWorldDirection);
        mirrorPortal.camera.transform.forward = mirrorPortal.transform.TransformDirection(playerLocalDirection);
    }
}
