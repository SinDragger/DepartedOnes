using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_MoveCamera : StoryScriptPlayerAction
{
    public override string processHead => "MoveCamera";
    public override void ProcessParamLine(string[] paramLine)
    {
        base.ProcessParamLine(paramLine);
    }
    public override bool ComfirmBlockEnd()
    {
        return base.ComfirmBlockEnd();
    }
}
