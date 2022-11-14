using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCubeBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Laser laser = new Laser();

    private bool refractionEnabled = false;

    private void Update()
    {
        if(!refractionEnabled) laser.DestroyCollider();
        laser.laser.enabled = refractionEnabled;
        refractionEnabled = false;
    }

    public void CreateRefraction()
    {
        if(refractionEnabled) return;
        laser.CreateCollider();
        refractionEnabled = true;
        laser.UpdateLaser();
    }
}
