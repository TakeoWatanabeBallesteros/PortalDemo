using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PedestalButtonBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Transform player;
    [SerializeField] 
    private Transform button;
    [SerializeField] 
    private InputActionReference interactAction;
    [SerializeField] 
    private Animator animator;
    [SerializeField] 
    private UnityEvent interact;
    
    
    private bool _enabled = true;

    private void OnEnable()
    {
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        interactAction.action.performed += ctx =>
        {
            if (!_enabled) return;
            animator.SetTrigger("active");
            interact.Invoke(); };
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector3.Distance(player.position, button.position) > 1.1f ||
             !(Vector3.Dot(player.forward.normalized, (button.position - player.position).normalized) > .99f)) && _enabled)
        {
            _enabled = false;
        }
        else if(!(Vector3.Distance(player.position, button.position) > 1.1f ||
                  !(Vector3.Dot(player.forward.normalized, (button.position - player.position).normalized) > .99f)) && !_enabled)
        {
            _enabled = true;
        }
    }
}
