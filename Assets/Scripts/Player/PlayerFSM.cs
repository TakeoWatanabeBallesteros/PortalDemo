using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FSM;

public class PlayerFSM : MonoBehaviour
{
    [Header("Inputs")] 
    [SerializeField] 
    private InputActionReference moveInput;
    [SerializeField] 
    private InputActionReference jumpInput;

    [Space] 
    [SerializeField] 
    private float speed;
    [SerializeField] 
    private float gravity;
    
    public bool grounded => controller.isGrounded;
    public Vector2 MoveInput => moveInput.action.ReadValue<Vector2>().normalized;

    private StateMachine fsm;
    private CollisionFlags collisionFlags;
    private CharacterController controller;
    private float verticalVelocity;
    
    private void OnEnable()
    {
        moveInput.action.Enable();
        jumpInput.action.Enable();
    }

    private void OnDisable()
    {
        moveInput.action.Disable();
        jumpInput.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        fsm = new StateMachine();
        controller = GetComponent<CharacterController>();
        
        AddStates();
        AddTransitions();
        
        fsm.SetStartState("Idle");
        fsm.Init();
    }
    // Update is called once per frame
    void Update()
    {
        fsm.OnLogic();
        // Always keep "pushing it" to maintain contact
        if (controller.isGrounded)  
            verticalVelocity = gravity;
        // Accelerate
        else
            verticalVelocity += gravity * Time.deltaTime;
        
        if ((collisionFlags & CollisionFlags.Above) != 0)
        {
            // Check if works
            verticalVelocity = 0.0f;
        }
        
        // normalise input direction
        Vector3 inputDirection = new Vector3(MoveInput.x, 0.0f, MoveInput.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (MoveInput != Vector2.zero)
        {
            // move
            inputDirection = transform.right * MoveInput.x + transform.forward * MoveInput.y;
        }
        // move the player
        collisionFlags =  controller.Move(inputDirection.normalized * (speed * Time.deltaTime) +
                                           new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void AddStates()
    {
        fsm.AddState("Idle", new Idle(this));
        fsm.AddState("Walk", new Walk(this));
        fsm.AddState("Run", new Run(this));
        fsm.AddState("Jump", new Jump(this));
        fsm.AddState("Fall", new Fall(this));
        fsm.AddState("Land", new Land(this));
    }

    private void AddTransitions()
    {
        fsm.AddTransition(new Transition("Idle", "Walk", t => moveInput.action.triggered));
        fsm.AddTransition(new Transition("Walk", "Idle", t => moveInput.action.triggered));
        fsm.AddTransitionFromAny(new Transition("", "Jump", t => jumpInput.action.triggered && grounded));
    }
}
