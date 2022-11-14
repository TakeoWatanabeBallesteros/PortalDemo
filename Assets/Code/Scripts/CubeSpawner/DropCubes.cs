using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropCubes : MonoBehaviour
{
    [SerializeField] 
    private Transform spawnPoint;
    [SerializeField] 
    private GameObject cubePrefab;
    
    private bool canSpawn = true;

    public void Spawn()
    {
        if(!canSpawn) return;
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
