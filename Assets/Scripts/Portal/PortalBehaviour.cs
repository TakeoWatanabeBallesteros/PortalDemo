using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Camera portalCamera;
    [SerializeField] 
    private PortalBehaviour mirrorPortal;
    [field:SerializeField] 
    public Transform PortalTransform { get; private set; }
    [SerializeField] 
    private Transform playerCamera;
    [SerializeField] 
    private Renderer Renderer;
    [field: SerializeField]
    public bool IsPlaced { get; private set; } = false;
    public List<PortalableObject> portalObjects = new List<PortalableObject>();
    public Collider wallCollider;

    private void OnEnable()
    {
        IsPlaced = true;
    }

    private void OnDisable()
    {
        IsPlaced = false;
    }
    
    private void Update()
    {
        Renderer.enabled = mirrorPortal.IsPlaced;
        
        for (int i = 0; i < portalObjects.Count; ++i)
        {
            Vector3 objPos = PortalTransform.InverseTransformPoint(portalObjects[i].transform.position);
            if (objPos.z > 0.0f)
            {
                portalObjects[i].Warp();
            }
        }
    }

    private void LateUpdate()
    {
        Quaternion direction = Quaternion.Inverse(transform.rotation) * playerCamera.rotation;
        mirrorPortal.portalCamera.transform.localEulerAngles = new Vector3(direction.eulerAngles.x,
                                                                         direction.eulerAngles.y + 180,
                                                                           direction.eulerAngles.z);
        Vector3 distance = transform.InverseTransformPoint(playerCamera.position);
        mirrorPortal.portalCamera.transform.localPosition = -new Vector3(distance.x, -distance.y, distance.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if (obj != null)
        {
            portalObjects.Add(obj);
            obj.SetIsInPortal(this, mirrorPortal, wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();

        if(portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            obj.ExitPortal(wallCollider);
        }
    }
}