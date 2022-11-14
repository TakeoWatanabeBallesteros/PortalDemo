using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropCubes : MonoBehaviour
{
    [SerializeField] 
    private Transform player;
    [SerializeField] 
    private Transform button;
    [SerializeField] 
    private Transform spawnPoint;
    [SerializeField] 
    private InputActionReference interactAction;
    [SerializeField] 
    private GameObject cubePrefab;
    
    private bool _enabled = true;
    private bool canSpawn = true;
    // Start is called before the first frame update
    private void OnEnable()
    {
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector3.Distance(player.position, button.position) > 1.1f ||
             !(Vector3.Dot(player.forward.normalized, (button.position - player.position).normalized) > .99f)) && _enabled)
        {
            interactAction.action.performed -= Spawn;
            _enabled = false;
        }
        else if(!(Vector3.Distance(player.position, button.position) > 1.1f ||
                  !(Vector3.Dot(player.forward.normalized, (button.position - player.position).normalized) > .99f)) && !_enabled)
        {
            if(!canSpawn) return;
            interactAction.action.performed += Spawn;
            _enabled = true;
        }
    }

    private void Spawn(InputAction.CallbackContext context)
    {
        Instantiate(cubePrefab, spawnPoint.position, spawnPoint.rotation);
        canSpawn = false;
        StartCoroutine(CoolDown());
    }

    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(3.0f);
        canSpawn = true;
    }
}
