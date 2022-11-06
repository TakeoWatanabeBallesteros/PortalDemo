using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    private Rigidbody rigidBody;
    private Transform pickPoint;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void Pick(Transform pickPoint)
    {
        this.pickPoint = pickPoint;
        transform.parent = pickPoint.transform;
        GetComponent<PortalableObject>().enabled = false;
        rigidBody.isKinematic = true;
        // rigidBody.useGravity = false;
        // rigidBody.drag = 10f;
    }

    public void Drop()
    {
        pickPoint = null;
        transform.parent = null;
        GetComponent<PortalableObject>().enabled = true;
        rigidBody.isKinematic = false;
        // rigidBody.useGravity = true;
        // rigidBody.drag = 1f;
    }
    
    private void Update()
    {
        if(pickPoint == null) return;
        if(Vector3.Distance(transform.position, pickPoint.position) > 0.1f)
            transform.position = Vector3.Lerp(transform.position, pickPoint.position, Time.deltaTime * 10f);
        
        rigidBody.velocity = Vector3.zero;
    }
}
