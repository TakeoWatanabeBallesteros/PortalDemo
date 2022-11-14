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
        laser.laser.enabled = refractionEnabled;
        refractionEnabled = false;
    }

    public void CreateRefraction()
    {
        if(refractionEnabled) return;
        refractionEnabled = true;
        laser.UpdateLaser();
    }
}
