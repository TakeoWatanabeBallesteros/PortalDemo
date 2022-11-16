using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPortable : MonoBehaviour
{
    [SerializeField] 
    private GameObject laserPrefab;
    [SerializeField] 
    private Laser myLaser;
    
    private PortalBehaviour inPortal;
    private PortalBehaviour outPortal;

    private GameObject portalLaser;
    private Laser laser;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);


    private void Update()
    {
        if(inPortal == null || outPortal == null) return;

        laser.UpdateLaser();
    }

    private void LateUpdate()
    {
        if(inPortal == null || outPortal == null) return;
        
        var inTransform = inPortal.transform;
        var outTransform = outPortal.transform;

        // Update position of clone.
        portalLaser.transform.position = myLaser.hitPoint;
        Vector3 relativePos = inTransform.InverseTransformPoint(portalLaser.transform.position);
        relativePos = halfTurn * relativePos;
        portalLaser.transform.position = outTransform.TransformPoint(relativePos);

        // Update rotation of clone.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        portalLaser.transform.rotation = outTransform.rotation * relativeRot;
            
        // Update scale of clone.
        if (laser.laser.startWidth * (outPortal.scale / inPortal.scale) <= 2 * 0.066f && 
            laser.laser.startWidth * (outPortal.scale / inPortal.scale) >= 0.5f * 0.066f)
        {
            laser.laser.startWidth *= (outPortal.scale / inPortal.scale);
            laser.laser.endWidth *= (outPortal.scale / inPortal.scale);
        }
    }

    public void SetIsInPortal(PortalBehaviour inPortal, PortalBehaviour outPortal)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;

        portalLaser = Instantiate(laserPrefab);
        laser = portalLaser.GetComponent<Laser>();
    }
    
    public void ExitPortal()
    {
        this.inPortal = null;
        this.outPortal = null;
        
        Destroy(portalLaser);
        laser = null;
    }
}
