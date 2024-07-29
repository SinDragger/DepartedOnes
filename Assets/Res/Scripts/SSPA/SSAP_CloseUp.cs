using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_CloseUp : StoryScriptPlayerAction
{
    public override string processHead => "CloseUp";

    public override void ProcessParamLine(string[] paramLine)
    {
        if (paramLine.Length > 1)
        {
            CameraControl.Instance.CloseUp(float.Parse(paramLine[1]));
        }
        else
        {
            CameraControl.Instance.CloseUp();
        }
    }
}
