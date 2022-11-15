using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LaserReceiverBehaviour : MonoBehaviour
{
    [SerializeField] 
    private UnityEvent active;
    [SerializeField] 
    private UnityEvent notActive;
    [SerializeField]
    private int objectsOnTop;
    [SerializeField] 
    private bool triggered;

    private List<Collider> others = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Laser")) return;
        objectsOnTop++;
        others.Add(other);
        if (objectsOnTop != 1) return;
        active?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Laser")) return;
        objectsOnTop--;
        others.Remove(other);
        if (objectsOnTop != 0) return;
        notActive?.Invoke();
    }

    private void Update()
    {
        foreach (var other in others.ToList().Where(other => !other))
        {
            objectsOnTop--;
            others.Remove(other);
            if (objectsOnTop != 0) return;
            notActive?.Invoke();
        }
    }
}
