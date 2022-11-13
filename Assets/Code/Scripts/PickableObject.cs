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
        rigidBody.useGravity = false;
        rigidBody.drag = 10f;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
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
        if (Vector3.Distance(transform.position, pickPoint.position) > 0.1f)
        {
            Vector3 direction = (pickPoint.position - transform.position);
            rigidBody.AddForce(direction * 50.0f);
        }
    }
}
