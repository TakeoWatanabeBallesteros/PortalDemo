using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField, Range(0, 0.1f)] 
    private float amplitude;
    [SerializeField, Range(0, 30)] 
    private float frequency;
    [Space] 
    [SerializeField] 
    private Transform _camera;
    [SerializeField] 
    private Transform cameraHolder;

    private float toggleSpeed = 3.0f;
    private Vector3 startPos;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        startPos = _camera.localPosition;
    }
    
    void Update()
    {
        CheckMotion();
        _camera.LookAt(FocusTarget());
    }

    private void CheckMotion()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        ResetPosition();
        if(speed < toggleSpeed) return;
        if(!controller.isGrounded) return;

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.deltaTime * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.deltaTime * frequency / 2) * amplitude * 2;
        return pos;
    }
    
    private void PlayMotion(Vector3 motion){
        _camera.localPosition += motion; 
    }

    private void ResetPosition()
    {
        if(_camera.localPosition == startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, startPos, 1 * Time.deltaTime);
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y,
            transform.position.z);
        pos += cameraHolder.forward * 15f;
        return pos;
    }
}
