using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_LookAtPos : StoryScriptPlayerAction
{
    public override string processHead => "LookAtPos";
    Vector3 pos;
    /// <summary>
    /// 0.LookAtPos
    /// 1.X,Y,Z
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {
        string[] posParam = paramLine[1].Split(',');
        pos = Vector3.zero;
        float.TryParse(posParam[0], out pos.x);
        float.TryParse(posParam[1], out pos.y);
        float.TryParse(posParam[2], out pos.z);
        CameraControl.Instance.LookAtV3(pos);
    }

    public override bool ComfirmBlockEnd()
    {
        return Vector3.Distance(CameraControl.Instance.transform.parent.position,pos)<0.05f;
    }

}
