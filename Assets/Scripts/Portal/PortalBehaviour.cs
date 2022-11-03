using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Camera portalCamera;
    [SerializeField] 
    private Transform otherPortalTransform;
    [SerializeField] 
    private PortalBehaviour mirrorPortal;
    [SerializeField] 
    private Transform playerPosition;
    [SerializeField] 
    private Transform playerCamera;
    [SerializeField] 
    private float offsetNearPlane;

    private void Start()
    {
        
    }

    
    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        /*// Takes the local position of the player from the other portal
        // and converts it to local position for my mirror portalCamera
        Vector3 playerWorldPosition = playerCamera.position;
        Vector3 playerLocalPosition = otherPortalTransform.InverseTransformPoint(playerWorldPosition);
        mirrorPortal.portalCamera.transform.position = mirrorPortal.transform.TransformPoint(playerLocalPosition);
        
        // Takes the local forward of the player from the other portal
        // and converts it to local forward for my mirror portalCamera
        Vector3 playerWorldDirection = playerCamera.transform.forward;
        Vector3 playerLocalDirection = otherPortalTransform.InverseTransformDirection(playerWorldDirection);
        mirrorPortal.portalCamera.transform.forward = mirrorPortal.transform.TransformDirection(playerLocalDirection);
        
        // The camera starts from the wall
        float distance = Vector3.Distance(mirrorPortal.portalCamera.transform.position,
            mirrorPortal.transform.position);
        mirrorPortal.portalCamera.nearClipPlane = distance + offsetNearPlane;*/
        
        Quaternion direction = Quaternion.Inverse(transform.rotation) * playerCamera.rotation;
        mirrorPortal.portalCamera.transform.localEulerAngles = new Vector3(direction.eulerAngles.x,
                                                                         direction.eulerAngles.y + 180,
                                                                           direction.eulerAngles.z);
        Vector3 distance = transform.InverseTransformPoint(playerCamera.transform.position);
        mirrorPortal.portalCamera.transform.localPosition = -new Vector3(distance.x, -distance.y, distance.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("ap");
        Vector3 l_Position =
            otherPortalTransform.transform.InverseTransformPoint(transform.position);
        Vector3 l_Direction =
            otherPortalTransform.transform.InverseTransformDirection(-transform.forward);
        playerPosition.position =
            mirrorPortal.transform.TransformPoint(l_Position);
        playerPosition.forward =
            mirrorPortal.transform.TransformDirection(l_Direction);
        playerPosition.position += playerPosition.forward * 0.3f;
    }
}
