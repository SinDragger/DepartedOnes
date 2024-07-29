using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_LookAtPos : CameraStatus
{
    Vector3 target;
    float speed = 3f;
    
    public CS_LookAtPos(Vector3 target)
    {
        this.target = target;
    }
    public override void Execute()
    {
        if (CParent.position == target)
            return;
        float distance = Vector3.Distance(CParent.position, target);
        float time = distance / smoothSpeed;
        if (distance < 0.05f)
        {
            CParent.position = target;
            CParent.transform.DOKill();
            return;
        }

        CParent.transform.DOMove(target, time).SetEase(Ease.Linear);
        //CParent.position = Vector3.Lerp(CParent.position, target, speed * Time.deltaTime);

    }


    public override void Exit()
    {


    }


}
