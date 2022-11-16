using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public Rigidbody rigidBody { get; private set; }
    public Transform pickPoint{ get; private set; }
    public Collider _collider { get; private set; }

    private Vector3 fw;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Pick(Transform pickPoint)
    {
        this.pickPoint = pickPoint;
        rigidBody.useGravity = false;
        rigidBody.drag = 10f;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        fw = pickPoint.InverseTransformDirection(transform.forward);
    }

    public void Drop()
    {
        pickPoint = null;
        rigidBody.useGravity = true;
        rigidBody.drag = 1f;
        rigidBody.constraints = RigidbodyConstraints.None;
    }
    
    private void Update()
    {
        if(pickPoint == null) return;
        if (!(Vector3.Distance(transform.position, pickPoint.position) > 0.3f))
        {
            rigidBody.velocity = Vector3.Slerp(rigidBody.velocity, Vector3.zero, Time.deltaTime*10f);
            return;
        }
        var direction = (pickPoint.position - transform.position).normalized;
        rigidBody.AddForce(direction * 65.0f);
    }

    private void FixedUpdate()
    {
        if(pickPoint == null) return;
        transform.forward =  Vector3.Slerp(transform.forward,pickPoint.forward, Time.deltaTime*7.0f);
    }
}
