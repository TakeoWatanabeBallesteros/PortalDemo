using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Laser
{
    [field:SerializeField] 
    public LineRenderer laser { get; private set; }
    [SerializeField] 
    private LayerMask layerMask;
    [SerializeField]
    private float maxLaserDistance;
    [SerializeField]
    private GameObject laserLimitCollider_Prefab;

    private GameObject laserLimitCollider;
    
    public void CreateCollider()
    {
        if (laserLimitCollider != null) return;
        laserLimitCollider = GameObject.Instantiate(laserLimitCollider_Prefab);
        laserLimitCollider.transform.parent = laser.transform;
        laserLimitCollider.transform.localPosition = Vector3.zero;
        laserLimitCollider.transform.localRotation = Quaternion.identity;
    }

    public void DestroyCollider()
    {
        if(laserLimitCollider == null) return;
        GameObject.Destroy(laserLimitCollider);
    }

    public void UpdateLaser()
    {
        Ray ray = new Ray(laser.transform.position, laser.transform.forward);
        float laserDistance = maxLaserDistance;
        if (Physics.Raycast(ray, out var rayCastHit, maxLaserDistance, layerMask.value))
        {
            laserDistance = Vector3.Distance(laser.transform.position, rayCastHit.point);
            if (rayCastHit.collider.CompareTag("RefractionCube"))
                rayCastHit.collider.GetComponent<RefractionCubeBehaviour>().CreateRefraction();
        }
        laser.SetPosition(1, new Vector3(0.0f, 0.0f,laserDistance));
        if(laserLimitCollider == null) return;
        laserLimitCollider.transform.localPosition = new Vector3(0.0f, 0.0f, laserDistance);
    }
}