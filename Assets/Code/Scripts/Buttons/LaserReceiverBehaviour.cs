using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserReceiverBehaviour : MonoBehaviour
{
    [SerializeField] 
    private UnityEvent active;
    [SerializeField] 
    private UnityEvent notActive;
    private int objectsOnTop;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Laser")) return;
        objectsOnTop++;
        if (objectsOnTop != 1) return;
        active?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Laser")) return;
        objectsOnTop--;
        if (objectsOnTop != 0) return;
        notActive?.Invoke();
    }
}
