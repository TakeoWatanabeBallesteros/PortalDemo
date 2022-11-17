using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour, IReset
{
    [SerializeField] 
    protected bool isRecursive;
    [SerializeField] 
    protected Camera portalCamera;
    [SerializeField] 
    protected PortalBehaviour mirrorPortal;
    [field:SerializeField] 
    public Transform PortalTransform { get; protected set; }
    [field:SerializeField] 
    public Animator PortalAnimator { get; protected set; }
    [SerializeField] 
    protected Transform playerCamera;
    [SerializeField] 
    protected Renderer Renderer;
    [field: SerializeField]
    public bool IsPlaced { get; protected set; } = false;
    protected List<PortalableObject> portalObjects = new List<PortalableObject>();
    public Collider wallCollider;
    
    protected Material material;
    [SerializeField]
    protected new Renderer renderer;

    public float scale;
    private RenderTexture tempTexture;

    private Vector3 startPosition;

    protected virtual void OnEnable()
    {
        if(!isRecursive)IsPlaced = true;
    }

    protected virtual void OnDisable()
    {
        IsPlaced = false;
    }

    protected virtual void Awake()
    {
        material = renderer.material;
        if (isRecursive) return;
        tempTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        material.mainTexture = tempTexture;
        portalCamera.targetTexture = tempTexture;

        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        Renderer.enabled = mirrorPortal.IsPlaced;
        
        foreach (var t in from t in portalObjects let objPos 
                     = PortalTransform.InverseTransformPoint(t.transform.position) where objPos.z > 0 select t)
        {
            t.Warp();
            if(!t.TryGetComponent<PickableObject>(out var comp)) return;
            comp.SwitchPickPoint(this, mirrorPortal);
        }
    }

    protected virtual void LateUpdate()
    {
        if(isRecursive) return;
        var inTransform = PortalTransform;
        var outTransform = mirrorPortal.PortalTransform;
        /*Quaternion direction = Quaternion.Inverse(transform.rotation) * playerCamera.rotation;
        mirrorPortal.portalCamera.transform.localEulerAngles = new Vector3(direction.eulerAngles.x,
                                                                         direction.eulerAngles.y + 180,
                                                                           direction.eulerAngles.z);
        Vector3 distance = transform.InverseTransformPoint(playerCamera.position);
        mirrorPortal.portalCamera.transform.localPosition = -new Vector3(distance.x, -distance.y, distance.z);*/
        // Position the camera behind the other portal.
        
        Vector3 relativePos = inTransform.InverseTransformPoint(playerCamera.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        portalCamera.transform.position = outTransform.TransformPoint(relativePos);

        // Rotate the camera to look through the other portal.
        Vector3 relativeRot = inTransform.InverseTransformDirection(playerCamera.forward);
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        portalCamera.transform.forward = outTransform.TransformDirection(relativeRot);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PortalableObject>(out var obj) && !portalObjects.Contains(obj))
        {   
            portalObjects.Add(obj);
            obj.SetIsInPortal(this, mirrorPortal, wallCollider);
        }
        else if (other.TryGetComponent<LaserPortable>(out var laser))
        {
            laser.SetIsInPortal(this, mirrorPortal);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PortalableObject>(out var obj) && portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            obj.ExitPortal(wallCollider);
        } 
        else if (other.TryGetComponent<LaserPortable>(out var laser) && (laser.myPortal == null ||laser.myPortal != this))
        {
            laser.ExitPortal();
        }
    }
    
    public void SetTexture(RenderTexture tex)
    {
        material.mainTexture = tex;
    }


    public bool IsRendererVisible()
    {
        return renderer.isVisible;
    }

    public void Placing()
    {
        IsPlaced = false;
    }

    public void Place()
    {
        IsPlaced = true;
    }

    public virtual void Reset()
    {
        transform.position = new Vector3(0,-10000,0); 
        portalObjects.Clear();
        IsPlaced = false;
        scale = 1;
    }
}