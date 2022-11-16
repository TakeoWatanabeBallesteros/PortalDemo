using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private List<IReset> resetObjects;
    private int checkPoinReference;
    private Vector3 currentCheckpointPos;
    private Quaternion currentCheckpointRot;
    private Transform player;
    
    //settings
    public float sensibility { get; private set; } = 10;
    
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static GameManager GetGameManager()
    {
        return instance == null ? null : instance;
    }
    
    public void SetPlayer(Transform player)
    {
        this.player = player;
        currentCheckpointPos = player.position;
        currentCheckpointRot = player.rotation;
        checkPoinReference = 0;
        resetObjects = new List<IReset>();
        resetObjects = AddResetObjects();
    }

    public void ResetGame()
    {
        foreach (var obj in resetObjects) {
            obj.Reset();
        }
        StartCoroutine(TeleportToCheckpoint());
        player.position = currentCheckpointPos;
        player.rotation = currentCheckpointRot;
    }

    private IEnumerator TeleportToCheckpoint()
    {
        yield return new WaitForFixedUpdate();
        player.position = currentCheckpointPos;
        player.rotation = currentCheckpointRot;
    }

    private static List<IReset> AddResetObjects()
    {
        var resetObj = FindObjectsOfType<MonoBehaviour>().OfType<IReset>();
        return resetObj.ToList();
    }
    
    public void SetCheckpoint(Transform checkpoint, int reference)
    {
        if(reference < checkPoinReference) return;
        currentCheckpointPos = checkpoint.position;
        currentCheckpointRot = checkpoint.rotation;
        checkPoinReference = reference;
    }

    public void SetSensibility(float sensibility)
    {
        this.sensibility = sensibility;
    }
}
