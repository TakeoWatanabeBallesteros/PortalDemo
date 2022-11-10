using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    [SerializeField] 
    private LineRenderer laser;
    [SerializeField] 
    private LayerMask layerMask;
    [SerializeField]
    private float maxLaserDistance;
    [SerializeField]
    private float alifeAngleInDegrees = 30.0f;

    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isAlive = Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(alifeAngleInDegrees * Mathf.Deg2Rad);
        laser.gameObject.SetActive(isAlive);
        if(!isAlive) return;
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
