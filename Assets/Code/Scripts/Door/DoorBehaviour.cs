using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : PortalBehaviour
{
    [SerializeField] 
    private Animator animator;

    private bool open = false;

    public void Open()
    {
        animator.SetTrigger("Open");
        open = true;
    }

    public void Close()
    {
        animator.SetTrigger("Close");
        open = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if(!open) return;
        base.OnTriggerEnter(other);
    }
}
