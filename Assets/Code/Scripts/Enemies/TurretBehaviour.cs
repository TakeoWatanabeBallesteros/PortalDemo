using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Laser laser;
    [SerializeField]
    private float alifeAngleInDegrees = 30.0f;

    // Update is called once per frame
    private void Update()
    {
        bool isAlive = Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(alifeAngleInDegrees * Mathf.Deg2Rad);
        laser.laser.enabled = isAlive;
        laser.laserLimitCollider.enabled = isAlive;
        if(!isAlive) return;
        laser.UpdateLaser();
    }
}