using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_ARPGModule : StoryScriptPlayerAction
{
    public override string processHead => "ARPGModule";

    public override void ProcessParamLine(string[] paramLine)
    {
        ARPGManager.Instance.Active();
    }

}
