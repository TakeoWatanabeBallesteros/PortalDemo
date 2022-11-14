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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("CompanionCube") || collision.gameObject.CompareTag("Player"))
        {
            objectsOnTop++;
            if(objectsOnTop == 1)
            {
                animator.SetTrigger("active");
                active?.Invoke();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("CompanionCube") || collision.gameObject.CompareTag("Player"))
        {
            objectsOnTop--;
            if(objectsOnTop == 0)
            {
                animator.SetTrigger("notActive");
                notActive?.Invoke();
            }
        }
    }
}
