using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FSM;

public class PlayerFSM : MonoBehaviour
{
    public bool what;
    [Header("Inputs")] 
    [SerializeField] 
    public InputActionReference moveInput;
    [SerializeField] 
    private InputActionReference jumpInput;
    [SerializeField] 
    private InputActionReference runInput;

    [Space] 
    [SerializeField] 
    public float walkSpeed;
    [SerializeField] 
    public float runSpeed;
    [SerializeField] 
    public float gravity;
    [SerializeField] 
    public float jumpHeight;
    
    public bool grounded => controller.isGrounded;
    public Vector2 MoveInput => moveInput.action.ReadValue<Vector2>().normalized;
    [HideInInspector]
    public float currentSpeed;

    private StateMachine fsm;
    [HideInInspector]
    public CollisionFlags collisionFlags;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public float verticalVelocity;
    
    private void OnEnable()
    {
        moveInput.action.Enable();
        runInput.action.Enable();
        jumpInput.action.Enable();
    }

    private void OnDisable()
    {
        moveInput.action.Disable();
        runInput.action.Disable();
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
        fsm.AddTwoWayTransition("Idle", "Walk", t => moveInput.action.ReadValue<Vector2>() != Vector2.zero && runInput.action.ReadValue<float>() == 0);
        fsm.AddTwoWayTransition("Idle", "Run", t => moveInput.action.ReadValue<Vector2>() != Vector2.zero && runInput.action.ReadValue<float>() > 0);
        fsm.AddTwoWayTransition("Walk", "Run", t => runInput.action.ReadValue<float>() > 0);
        fsm.AddTransitionFromAny(new Transition("", "Jump", t => jumpInput.action.triggered && grounded));
        fsm.AddTransition("Jump", "Fall", t => verticalVelocity <= 0);
        fsm.AddTransition("Fall", "Land", t => grounded);
    }
}
