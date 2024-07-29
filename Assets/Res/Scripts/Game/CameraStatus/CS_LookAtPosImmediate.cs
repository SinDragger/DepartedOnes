using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_LookAtPosImmediate : CameraStatus
{
    Vector3 target;
    float speed = 3f;
    
    public CS_LookAtPosImmediate(Vector3 target)
    {
        this.target = target;
    }
    public override void Execute()
    {
        CameraControl.Instance.SetCameraPos(target.x,target.z);
        //CParent.position = Vector3.Lerp(CParent.position, target, speed * Time.deltaTime);

    }


    public override void Exit()
    {


    }


}
