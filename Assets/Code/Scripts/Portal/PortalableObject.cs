using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalableObject : MonoBehaviour
{
    [SerializeField]
    protected GameObject cloneObject;

    private int inPortalCount = 0;

    private PortalBehaviour inPortal;
    private PortalBehaviour outPortal;

    private new Rigidbody rigidbody;
    
    [SerializeField]
    private new Collider collider;

    protected Collider wallCollider;

    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private CharacterController controller;

    protected virtual void Awake()
    {
        cloneObject = Instantiate(cloneObject);
        cloneObject.SetActive(false);
        cloneObject.transform.localScale = transform.localScale;

        rigidbody = GetComponent<Rigidbody>();
        if(collider == null) collider = GetComponent<Collider>(); 
        controller = GetComponent<CharacterController>();
    }

    protected virtual void LateUpdate()
    {
        if(inPortal == null || outPortal == null) return;

        if(cloneObject.activeSelf && inPortal.IsPlaced && outPortal.IsPlaced)
        {
            var inTransform = inPortal.transform;
            var outTransform = outPortal.transform;

            // Update position of clone.
            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = halfTurn * relativePos;
            cloneObject.transform.position = outTransform.TransformPoint(relativePos);

            // Update rotation of clone.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
            relativeRot = halfTurn * relativeRot;
            cloneObject.transform.rotation = outTransform.rotation * relativeRot;
            
            // Update scale of clone.
            if(cloneObject.transform.localScale.x * (outPortal.scale / inPortal.scale) <= 2 && 
               cloneObject.transform.localScale.x * (outPortal.scale / inPortal.scale) >= 0.5f) 
                cloneObject.transform.localScale *= (outPortal.scale / inPortal.scale);
        }
        else
        {
            // cloneObject.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
        }
    }

    public void SetIsInPortal(PortalBehaviour inPortal, PortalBehaviour outPortal, Collider wallCollider)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;
        this.wallCollider = wallCollider;
        
        Physics.IgnoreLayerCollision(gameObject.layer, wallCollider.gameObject.layer, true);

        cloneObject.SetActive(true);

        ++inPortalCount;
    }

    public void ExitPortal(Collider wallCollider)
    {
        Physics.IgnoreLayerCollision(gameObject.layer, wallCollider.gameObject.layer, false);
        --inPortalCount;

        if (inPortalCount == 0)
        {
            cloneObject.SetActive(false);
        }
    }

    public virtual void Warp()
    {
        var inTransform = inPortal.PortalTransform;
        var outTransform = outPortal.PortalTransform;

        if(controller != null) controller.enabled = false;
        Vector3 relativeVel = Vector3.zero;
        if (rigidbody != null)
        {
            
            relativeVel = inTransform.InverseTransformDirection(rigidbody.velocity);
            rigidbody.isKinematic = true;
        }
        
        // Update position of object.
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        relativePos = halfTurn * relativePos;
        transform.position = outTransform.TransformPoint(relativePos);

        // Update rotation of object.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        transform.rotation = outTransform.rotation * relativeRot;
        
        // Update scale of object
        if(transform.localScale.x * (outPortal.scale / inPortal.scale) <= 2 && transform.localScale.x * (outPortal.scale / inPortal.scale) >= 0.5f)
            transform.localScale *= (outPortal.scale / inPortal.scale);
    
        if(controller != null) controller.enabled = true;
        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;
            // Update velocity of rigidbody.
            relativeVel = halfTurn * relativeVel;
            rigidbody.velocity = outTransform.TransformDirection(relativeVel);
        }
        
        // Swap portal references.
        (inPortal, outPortal) = (outPortal, inPortal);
    }
}