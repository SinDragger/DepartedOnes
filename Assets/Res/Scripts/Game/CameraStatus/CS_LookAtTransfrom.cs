using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_LookAtTransfrom : CameraStatus
{
    Transform target;
    float speed;
    Vector3 deviation;
    public CS_LookAtTransfrom(Transform target)
    {
        this.target = target;
        deviation = Vector3.zero;
    }
    public CS_LookAtTransfrom(Transform target, Vector3 deviation,float speed=0)
    {
        this.target = target;
        this.deviation = deviation;
        if (speed != 0) { 
        this.speed = speed;
        }
    }
    public override void Execute()
    {



        float distance = Vector3.Distance(CParent.position, target.position + deviation);
        float time = distance / smoothSpeed;
        if (distance < 0.05f)
        {
            CParent.position = target.position + deviation;
            CParent.transform.DOKill();
            return;
        }

        CParent.transform.DOMove(target.position + deviation, time).SetEase(Ease.Linear);
        //CParent.position = Vector3.Lerp(CParent.position, target, speed * Time.deltaTime);
       


    }

    public override void Exit()
    {



    }

}
