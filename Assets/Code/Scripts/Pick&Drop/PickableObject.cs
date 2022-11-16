using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour, IReset
{
    public Rigidbody rigidBody { get; private set; }
    public Transform pickPoint{ get; private set; }
    public Collider _collider { get; private set; }

    private Transform playerFroward;

    private Vector3 fw;
    
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void Pick(Transform pickPoint, Transform playerForward)
    {
        this.pickPoint = pickPoint;
        this.playerFroward = playerForward;
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
        if (!(Vector3.Distance(transform.position, pickPoint.position) > 0.1f))
        {
            rigidBody.velocity = Vector3.Slerp(rigidBody.velocity, Vector3.zero, Time.deltaTime*10f);
            return;
        }
        var direction = (pickPoint.position - transform.position);
        rigidBody.AddForce(direction * 80.0f);
    }

    private void FixedUpdate()
    {
        if(pickPoint == null) return;
        transform.forward =  Vector3.Slerp(transform.forward,playerFroward.forward, Time.deltaTime*7.0f);
    }

    public void Reset()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale /= transform.localScale.x;
    }
}
