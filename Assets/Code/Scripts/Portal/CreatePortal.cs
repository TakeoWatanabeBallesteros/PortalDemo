using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreatePortal : MonoBehaviour
{
    [SerializeField] 
    public InputActionReference bluePortalShoot;
    [SerializeField] 
    public InputActionReference orangePortalShoot;
    [SerializeField] 
    public InputActionReference changeScaleInput;
    [SerializeField] 
    private Transform head;
    [SerializeField] 
    public PortalBehaviour bluePortal;
    [SerializeField] 
    public PortalBehaviour orangePortal;
    [SerializeField] 
    private GameObject portal;
    [SerializeField] 
    private GameObject portalImage;
    [SerializeField]
    private Transform[] validPoints;
    [SerializeField] 
    private LayerMask portalLayerMask;
    [SerializeField] 
    private LayerMask pointsLayerMask;
    [SerializeField] 
    private float minValidDistance;
    [SerializeField] 
    private float maxValidDistance;
    [SerializeField] 
    private float minDotAngle;
    
    private Vector3 position;
    private Vector3 normal;
    private Collider wallCollider;
    private float PortalsScale = 1.0f;

    private int AnimationScaleID;

    private void OnEnable()
    {
        bluePortalShoot.action.Enable();
        orangePortalShoot.action.Enable();
        changeScaleInput.action.Enable();
    }

    private void OnDisable()
    {
        bluePortalShoot.action.Disable();
        orangePortalShoot.action.Disable();
        changeScaleInput.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        AnimationScaleID = Animator.StringToHash("Scale");
        bluePortal.PortalAnimator.SetFloat(AnimationScaleID, PortalsScale);
        orangePortal.PortalAnimator.SetFloat(AnimationScaleID, PortalsScale);
        bluePortalShoot.action.performed += ctx => _CreatePortal(ctx, 1);
        orangePortalShoot.action.performed += ctx => _CreatePortal(ctx, 2);
        changeScaleInput.action.performed += ScalePortals;
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerFSM.canShoot) return;
        portalImage.SetActive(IsValidPosition(head.position, head.forward, 100, out position, out normal));
    }
    
    public bool IsValidPosition(Vector3 startPosition, Vector3 forward, float maxDistance, out Vector3 position, out Vector3 normal)
    {
        Ray ray = new Ray(startPosition, forward);
        position = Vector3.zero;
        normal = Vector3.forward;
        portal.transform.position = position;
        portal.transform.rotation = Quaternion.LookRotation(normal);

        if (Physics.Raycast(ray, out var hitInfo, maxDistance, portalLayerMask.value))
        {
            if (hitInfo.collider.CompareTag("DrawableWall"))
            {
                normal = hitInfo.normal;
                position = hitInfo.point;
                wallCollider = hitInfo.collider;
                portal.transform.position = position;
                portal.transform.rotation = Quaternion.LookRotation(normal);
                foreach (var direction in validPoints.Select(point => point.position - startPosition))
                {
                    direction.Normalize();
                    ray = new Ray(startPosition, direction);
                    if (Physics.Raycast(ray, out var pointHit, maxDistance, pointsLayerMask.value))
                    {
                        if (pointHit.collider.CompareTag("DrawableWall"))
                        {
                            float distance = Vector3.Distance(position, pointHit.point);
                            float dotAngle = Vector3.Dot(normal, pointHit.normal);
                            // if one of the conditions is false the portal cant be there
                            if (!(distance >= minValidDistance && distance <= maxValidDistance && 
                                  dotAngle > minDotAngle))
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }

    private void _CreatePortal(InputAction.CallbackContext context, int portal)
    {
        if(!IsValidPosition(Camera.main.transform.position, Camera.main.transform.forward, 100, out position, out normal))
            return;
        if(!PlayerFSM.canShoot) return;
        switch (portal)
        {
            case 1:
                bluePortal.gameObject.SetActive(true);
                bluePortal.PortalAnimator.SetTrigger("Create");
                bluePortal.wallCollider = wallCollider;
                bluePortal.transform.position = position;
                bluePortal.transform.rotation = Quaternion.LookRotation(normal);
                bluePortal.PortalAnimator.SetFloat(AnimationScaleID, PortalsScale);
                bluePortal.scale = PortalsScale;
                break;
            case 2:
                orangePortal.gameObject.SetActive(true);
                orangePortal.PortalAnimator.SetTrigger("Create");
                orangePortal.wallCollider = wallCollider;
                orangePortal.transform.position = position;
                orangePortal.transform.rotation = Quaternion.LookRotation(normal);
                orangePortal.PortalAnimator.SetFloat(AnimationScaleID, PortalsScale);
                orangePortal.scale = PortalsScale;
                break;
        }
    }

    private void ScalePortals(InputAction.CallbackContext context)
    {
        PortalsScale = PortalsScale switch
        {
            0.5f => 1,
            1 => 2,
            2 => 0.5f,
            _ => PortalsScale
        };
        portalImage.transform.parent.localScale = new Vector3(PortalsScale, PortalsScale, PortalsScale);
    }
}
