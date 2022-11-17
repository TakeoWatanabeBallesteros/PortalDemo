using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Laser : MonoBehaviour
{
    [field:SerializeField] 
    public LineRenderer laser { get; private set; }
    [SerializeField] 
    private LayerMask layerMask;
    [SerializeField]
    private float maxLaserDistance;
    [field:SerializeField]
    public SphereCollider laserLimitCollider { get; private set; }
    public Vector3 hitPoint { get; private set; }
    public Vector3 hitAngle { get; private set; }

    public float size = 2;
    public void UpdateLaser()
    {
        Ray ray = new Ray(laser.transform.position, laser.transform.forward);
        float laserDistance = maxLaserDistance;
        if (Physics.Raycast(ray, out var rayCastHit, maxLaserDistance, layerMask.value))
        {
            laserDistance = Vector3.Distance(laser.transform.position, rayCastHit.point);
            hitPoint = rayCastHit.point;
            if (rayCastHit.collider.CompareTag("RefractionCube"))
                rayCastHit.collider.GetComponent<RefractionCubeBehaviour>().CreateRefraction();
        }
        laser.SetPosition(1, new Vector3(0.0f, 0.0f,laserDistance));
        laserLimitCollider.center = new Vector3(0.0f, 0.0f, laserDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.GetGameManager().reset = true;
        }
    }
}