using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_PlayCharacterAnim : StoryScriptPlayerAction
{
    public override string processHead => "PlayCharacterAnim";
    public override void ProcessParamLine(string[] paramLine)
    {
        base.ProcessParamLine(paramLine);
    }

    public override bool ComfirmBlockEnd()
    {
        return base.ComfirmBlockEnd();
    }
}
