using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLaser : MonoBehaviour
{
    [SerializeField] 
    private Laser laser = new Laser();

    private void OnEnable()
    {
        laser.CreateCollider();
    }

    private void OnDisable()
    {
        laser.DestroyCollider();
    }

    private void Update()
    {
       laser.UpdateLaser(); 
    }
}
