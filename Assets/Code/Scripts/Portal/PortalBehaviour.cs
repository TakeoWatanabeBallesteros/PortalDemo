using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
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
    }

    protected virtual void Update()
    {
        Renderer.enabled = mirrorPortal.IsPlaced;
        
        for (int i = 0; i < portalObjects.Count; ++i)
        {
            Vector3 objPos = PortalTransform.InverseTransformPoint(portalObjects[i].transform.position);
            if (objPos.z > 0.0f)
            {
                if(!portalObjects[i].onHold)portalObjects[i].Warp();
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if(isRecursive) return;
        Quaternion direction = Quaternion.Inverse(transform.rotation) * playerCamera.rotation;
        mirrorPortal.portalCamera.transform.localEulerAngles = new Vector3(direction.eulerAngles.x,
                                                                         direction.eulerAngles.y + 180,
                                                                           direction.eulerAngles.z);
        Vector3 distance = transform.InverseTransformPoint(playerCamera.position);
        mirrorPortal.portalCamera.transform.localPosition = -new Vector3(distance.x, -distance.y, distance.z);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if (obj != null && !portalObjects.Contains(obj))
        {   
            portalObjects.Add(obj);
            obj.SetIsInPortal(this, mirrorPortal, wallCollider);
        }else if (other.TryGetComponent<LaserPortable>(out var laser))
        {
            //laser.SetIsInPortal(this, mirrorPortal, wallCollider);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        PortalableObject portable = null;

        if (!portalObjects.Contains(obj) || obj.onHold) return;
        portalObjects.Remove(obj);
        obj.ExitPortal(wallCollider);
        if (!obj.TryGetComponent<PickUpDrop>(out var item)) return;
        if (item.pickableObject == null || !item.pickableObject.TryGetComponent<PortalableObject>(out portable)) return;
        portable.onHold = false;
        portable.Warp();
        portable.onHold = true;
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

    public void ExitPortal(PortalableObject obj)
    {
        if (!portalObjects.Contains(obj)) return;
        portalObjects.Remove(obj);
        obj.ExitPortal(wallCollider);
    }
}