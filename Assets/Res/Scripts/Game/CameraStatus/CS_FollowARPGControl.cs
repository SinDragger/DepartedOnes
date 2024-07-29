using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_FollowARPGControl : CameraStatus
{
    ARPGControl target;

    public CS_FollowARPGControl(ARPGControl target)
    {
        this.target = target;

    }
    public override void Execute()
    {
        
        CameraControl.Instance.SetTargetMoveTo(target.targetMoveTo);
        //CParent.position += CameraControl.Instance.Fix();
        CParent.position = CameraControl.Instance.Fix();
    }

    public override void Exit()
    {

    }
}
