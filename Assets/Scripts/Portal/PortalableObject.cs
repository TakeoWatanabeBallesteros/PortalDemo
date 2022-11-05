using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject cloneObject;

    private int inPortalCount = 0;
    
    protected PortalBehaviour inPortal;
    private PortalBehaviour outPortal;

    private new Rigidbody rigidbody;
    protected new Collider collider;

    protected Collider wallCollider;

    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private CharacterController controller;

    protected virtual void Awake()
    {
        cloneObject.SetActive(false);
        cloneObject.transform.localScale = transform.localScale;

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>(); 
        controller = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        if(inPortal == null || outPortal == null)
        {
            return;
        }

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
        }
        else
        {
            cloneObject.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
        }
    }

    public void SetIsInPortal(PortalBehaviour inPortal, PortalBehaviour outPortal, Collider wallCollider)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;
        this.wallCollider = wallCollider;
        
        Physics.IgnoreCollision(collider, wallCollider);

        cloneObject.SetActive(false);

        ++inPortalCount;
    }

    public void ExitPortal(Collider wallCollider)
    {
        Physics.IgnoreCollision(collider, wallCollider, false);
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
        
        // Update position of object.
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        //if (relativePos.z != 0.0) return;
        relativePos = halfTurn * relativePos;
        // Debug.Log(Vector3.Distance(outTransform.TransformPoint(relativePos), outTransform.position));
        if(Vector3.Distance(outTransform.TransformPoint(relativePos), outTransform.position) > 2.0f) return;
        transform.position = outTransform.TransformPoint(relativePos);

        // Update rotation of object.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        transform.rotation = outTransform.rotation * relativeRot;
    
        if(controller != null) controller.enabled = true;
        
        
        // Swap portal references.
        (inPortal, outPortal) = (outPortal, inPortal);
    }
}