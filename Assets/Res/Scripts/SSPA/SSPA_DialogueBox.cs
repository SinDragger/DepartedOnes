using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_DialogueBox : StoryScriptPlayerAction
{
    public override string processHead => "DialogueBox";

  
    /// <summary>
    /// 0 DialogueBox
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {
        DialogManager.instance.ActiveBox();
    }

    

}
