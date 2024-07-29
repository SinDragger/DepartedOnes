using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_SetSpeakerName : StoryScriptPlayerAction
{
    public override string processHead => "SetSpeakerName";

    public override void ProcessParamLine(string[] paramLine)
    {
        DialogManager.instance.SetBoxName(paramLine[1]);
    }

}
