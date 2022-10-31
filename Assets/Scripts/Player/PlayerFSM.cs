using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class PlayerFSM : MonoBehaviour
{
    [SerializeField] 
    private bool grounded;
    [SerializeField] 
    private float gravity;
    
    private CollisionFlags collisionFlags;
    private CharacterController controller;
    private float verticalVelocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        // Always keep "pushing it" to maintain contact
        if (controller.isGrounded)  
            verticalVelocity = gravity;
        // Accelerate
        else
            verticalVelocity += gravity * Time.deltaTime;
        
        controller.Move(transform.forward*0.01f + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}
