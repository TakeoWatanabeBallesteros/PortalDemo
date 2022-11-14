using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorButtonBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Animator animator;
    [SerializeField] 
    private UnityEvent active;
    [SerializeField] 
    private UnityEvent notActive;
    private int objectsOnTop;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("CompanionCube") && !other.gameObject.CompareTag("Player")) return;
        objectsOnTop++;
        if (objectsOnTop != 1) return;
        animator.SetTrigger("active");
        active?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("CompanionCube") && !other.gameObject.CompareTag("Player")) return;
        objectsOnTop--;
        if (objectsOnTop != 0) return;
        animator.SetTrigger("notActive");
        notActive?.Invoke();
    }
}
