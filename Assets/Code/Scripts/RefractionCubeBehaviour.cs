using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCubeBehaviour : MonoBehaviour
{
    [SerializeField] 
    private LineRenderer laser;
    [SerializeField] 
    private LayerMask layerMask;
    [SerializeField]
    private float maxLaserDistance;
    [SerializeField]
    private float alifeAngleInDegrees = 30.0f;

    private bool refractionEnabled = false;

    private void Update()
    {
        laser.gameObject.SetActive(refractionEnabled);
        refractionEnabled = false;
    }

    public void CreateRefraction()
    {
        if(refractionEnabled) return;
        refractionEnabled = true;
        Ray ray = new Ray(laser.transform.position, laser.transform.forward);
        float laserDistance = maxLaserDistance;
        if (Physics.Raycast(ray, out var rayCastHit, maxLaserDistance, layerMask.value))
        {
            laserDistance = Vector3.Distance(laser.transform.position, rayCastHit.transform.position);
            if (rayCastHit.collider.CompareTag("RefractionCube"))
                rayCastHit.collider.GetComponent<RefractionCubeBehaviour>().CreateRefraction();
        }
        laser.SetPosition(1, new Vector3(0.0f, 0.0f,laserDistance));
    }
}
