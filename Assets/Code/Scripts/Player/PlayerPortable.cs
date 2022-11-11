using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortable : PortalableObject
{
    private PlayerLook cameraMove;
    private Transform cloneHeadTransform;
    
    protected override void Awake()
    {
        base.Awake();

        cameraMove = GetComponent<PlayerLook>();
        cloneHeadTransform = cloneObject.transform.GetChild(0);
    }

    protected override void LateUpdate()
    {
        cloneHeadTransform.localRotation = Quaternion.Slerp(cloneHeadTransform.localRotation, Quaternion.Euler(cameraMove.GetPitch(), 0.0f, 0.0f), Time.deltaTime * 15.0f);
        base.LateUpdate();
    }

    public override void Warp()
    {
        base.Warp();
        cameraMove.ResetTargetRotation();
    }
}