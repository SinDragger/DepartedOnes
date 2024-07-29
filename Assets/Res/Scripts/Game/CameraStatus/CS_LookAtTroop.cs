using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_LookAtTroop : CameraStatus
{
    Vector3 target;
    float speed = 100f;

    public CS_LookAtTroop(Vector3 target)
    {
        this.target = target;
    }
    public override void Execute()
    {
        if (CParent.position == target) {
            Exit();
            return;
        }
            
        float distance = Vector3.Distance(CParent.position, target);
        float time = distance / speed;
        if (distance < 0.05f)
        {
            CParent.position = target;
            CParent.transform.DOKill();
        }

        CParent.transform.DOMove(target, time).SetEase(Ease.Linear);
        //CParent.position = Vector3.Lerp(CParent.position, target, speed * Time.deltaTime);

    }
    public override void Exit()
    {
       CameraControl.Instance.status=new CS_FreeMode();
    }
}
