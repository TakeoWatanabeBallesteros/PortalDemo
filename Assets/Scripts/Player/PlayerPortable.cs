using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortable : PortalableObject
{
    private PlayerLook cameraMove;

    protected override void Awake()
    {
        base.Awake();

        cameraMove = GetComponent<PlayerLook>();
    }

    public override void Warp()
    {
        Debug.Log("Teleport :)");
        base.Warp();
        cameraMove.ResetTargetRotation();
    }
}