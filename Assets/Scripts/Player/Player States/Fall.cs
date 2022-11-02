using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Fall : StateBase
{
    private PlayerFSM playerFsm;

    public Fall(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this.playerFsm = fsm;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnLogic()
    {
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
