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

    private List<Collider> others = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Laser")) return;
        others.Add(other);
        if (others.Count != 1) return;
        active?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Laser")) return;
        others.Remove(other);
        if (others.Count != 0) return;
        notActive?.Invoke();
    }

    private void Update()
    {
        foreach (var other in others.ToList().Where(other => !other.enabled))
        {
            others.Remove(other);
            if (others.Count != 0) return;
            notActive?.Invoke();
        }
    }
}
