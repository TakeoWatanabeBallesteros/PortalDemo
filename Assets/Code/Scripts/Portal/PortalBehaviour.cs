using System;
using System.Collections;
using System.Collections.Generic;
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
        
        for (int i = 0; i < portalObjects.Count; ++i)
        {
            Vector3 objPos = PortalTransform.InverseTransformPoint(portalObjects[i].transform.position);
            if (objPos.z > 0)
            {
                portalObjects[i].Warp();
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

    public void Reset()
    {
        startPosition = transform.position;
        portalObjects.Clear();
        IsPlaced = false;
        scale = 1;
    }
}