using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour, IReset
{
    public Rigidbody rigidBody { get; private set; }
    public Transform pickPoint{ get; private set; }
    private Transform pickPointPortal;
    public Collider _collider { get; private set; }

    private Transform playerFroward;
    private Transform playerForwardPortal;

    private Vector3 fw;
    
    private Vector3? startPosition = null;
    private Quaternion startRotation;

    private PortalBehaviour inPortal;
    private PortalBehaviour outPortal;
    private GameObject ap;
    private PickUpDrop father;

    public bool Spawned = false;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Start()
    {
        if (!Spawned) return; 
        GameManager.GetGameManager().AddResetObject(this);
    }

    public void Pick(Transform pickPoint, Transform playerForward, PickUpDrop father)
    {
        this.father = father;
        this.pickPoint = pickPoint;
        this.playerFroward = playerForward;
        rigidBody.useGravity = false;
        rigidBody.drag = 10f;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        fw = pickPoint.InverseTransformDirection(transform.forward);
    }

    public void Drop()
    {
        father = null;
        pickPoint = null;
        rigidBody.useGravity = true;
        rigidBody.drag = 1f;
        rigidBody.constraints = RigidbodyConstraints.None;
        if(inPortal != null) ExitPortal();
    }
    
    private void Update()
    {
        var direction = Vector3.zero;
        if(pickPoint == null) return;
        if (inPortal != null)
        {
            if (Vector3.Dot(inPortal.transform.forward, playerFroward.forward) > 0 || Vector3.Distance(inPortal.transform.position, playerFroward.position) > 2.7f)
            {
                father.Detach();
                father = null;
                return;
            }
            pickPointPortal.position = pickPoint.position;
            pickPointPortal.rotation = pickPoint.rotation;
            var inTransform = inPortal.transform;
            var outTransform = outPortal.transform;

            // Update position of clone.
            Vector3 relativePos = inTransform.InverseTransformPoint(pickPoint.position);
            relativePos = halfTurn * relativePos;
            pickPointPortal.transform.position = outTransform.TransformPoint(relativePos);

            // Update rotation of clone.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * pickPoint.rotation;
            relativeRot = halfTurn * relativeRot;
            pickPointPortal.transform.rotation = outTransform.rotation * relativeRot;
            
            if (!(Vector3.Distance(transform.position, pickPointPortal.position) > 0.1f))
            {
                rigidBody.velocity = Vector3.Slerp(rigidBody.velocity, Vector3.zero, Time.deltaTime*10f);
                return;
            }
            
            direction = (pickPointPortal.position - transform.position);
            rigidBody.AddForce(direction * 80.0f);
            ap.transform.position = pickPointPortal.position;
            return;
        }
        if (!(Vector3.Distance(transform.position, pickPoint.position) > 0.1f))
        {
            rigidBody.velocity = Vector3.Slerp(rigidBody.velocity, Vector3.zero, Time.deltaTime*10f);
            return;
        }
        direction = (pickPoint.position - transform.position);
        rigidBody.AddForce(direction * 80.0f);
    }

    private void FixedUpdate()
    {
        if(pickPoint == null) return;
        if (inPortal != null)
        {
            transform.forward =  Vector3.Slerp(transform.forward,outPortal.transform.forward, Time.deltaTime*7.0f);
            return;
        }
        transform.forward =  Vector3.Slerp(transform.forward,playerFroward.forward, Time.deltaTime*7.0f);
    }

    public void Reset()
    {
        if (Spawned)
        {
            Destroy(gameObject);
            GameManager.GetGameManager().RemoveResetObject(this);
            return;
        }
        transform.position = (Vector3)startPosition;
        transform.rotation = startRotation;
        transform.localScale /= transform.localScale.x;
        pickPoint = null;
        father = null;
        inPortal = null;
        outPortal = null;
        if(ap == null) return;
        Destroy(ap);
    }

    public void SwitchPickPoint(PortalBehaviour inPortal, PortalBehaviour outPortal)
    {
        if(pickPoint == null) return;
        if (this.inPortal != null)
        {
            inPortal = null;
            outPortal = null;
            Destroy(ap);
        }
        this.inPortal = inPortal;
        this.outPortal = outPortal;
        ap = new GameObject("ap");
        pickPointPortal = ap.transform;
    }
    
    public void ExitPortal()
    {
        inPortal = null;
        outPortal = null;
        Destroy(ap);
    }
}
