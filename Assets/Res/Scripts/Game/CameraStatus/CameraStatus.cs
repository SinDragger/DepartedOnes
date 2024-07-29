using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CS_表示CameraStatus
/// </summary>
public class CameraStatus
{
    protected float smoothSpeed
    {
        get { return CameraControl.Instance.smoothSpeed; }

    }
  protected  Transform CParent
    {
        get { return CameraControl.Instance.transform.parent; }
    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {

    }
}
