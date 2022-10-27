using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

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
    private float offsetNearPlane;
    [SerializeField]
    private List<Transform> validPoints;
    [SerializeField] 
    private float minValidDistance;
    [SerializeField] 
    private float maxValidDistance;
    [SerializeField] 
    private float minDotAngle;
    
    
    private void Start()
    {
        
    }

    
    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        // Takes the local position of the player from the other portal
        // and converts it to local position for my mirror portalCamera
        Vector3 playerWorldPosition = playerPosition.position;
        Vector3 playerLocalPosition = otherPortalTransform.InverseTransformPoint(playerWorldPosition);
        mirrorPortal.portalCamera.transform.position = mirrorPortal.transform.TransformPoint(playerLocalPosition);
        
        // Takes the local forward of the player from the other portal
        // and converts it to local forward for my mirror portalCamera
        Vector3 playerWorldDirection = Camera.main.transform.forward;
        Vector3 playerLocalDirection = otherPortalTransform.InverseTransformDirection(playerWorldDirection);
        mirrorPortal.portalCamera.transform.forward = mirrorPortal.transform.TransformDirection(playerLocalDirection);

        float distance = Vector3.Distance(mirrorPortal.portalCamera.transform.position,
            mirrorPortal.transform.position);
        mirrorPortal.portalCamera.nearClipPlane = distance + offsetNearPlane;
    }

    public bool IsValidPosition(Vector3 startPosition, Vector3 forward, float maxDistance, LayerMask portalLayerMask, out Vector3 position, out Vector3 normal)
    {
        Ray ray = new Ray(startPosition, forward);
        RaycastHit hitInfo;
        position = Vector3.zero;
        normal = Vector3.forward;
        
        if (Physics.Raycast(ray, out hitInfo, maxDistance, portalLayerMask.value))
        {
            if (hitInfo.collider.tag == "DrawableWall")
            {
                normal = hitInfo.normal;
                position = hitInfo.point;

                foreach (var point in validPoints)
                {
                    Vector3 direction = point.position - startPosition;
                    direction.Normalize();
                    ray = new Ray(startPosition, direction);
                    if (Physics.Raycast(ray, out hitInfo, maxDistance, portalLayerMask.value))
                    {
                        if (hitInfo.collider.tag == "DrawableWall")
                        {
                            float distance = Vector3.Distance(position, hitInfo.point);
                            float dotAngle = Vector3.Dot(normal, hitInfo.normal);
                            if (!(distance >= minValidDistance && distance <= maxValidDistance &&
                                  dotAngle > minDotAngle))
                                return false;
                        }

                        return true;
                    }

                    return false;
                }
            }
            return false;
        }
        return true;
    }
}
