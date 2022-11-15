using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBobController : MonoBehaviour
{
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public CharacterController controller;
    public Transform head;
    public PlayerFSM fsm;
    public InputActionReference move;

    float defaultPosY = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosY = head.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(move.action.ReadValue<Vector2>().magnitude > 0.1 && fsm.grounded)
        {
            //Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            head.localPosition = new Vector3(head.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, head.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            head.localPosition = new Vector3(head.localPosition.x, Mathf.Lerp(head.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), head.localPosition.z);
        }
    }
}
