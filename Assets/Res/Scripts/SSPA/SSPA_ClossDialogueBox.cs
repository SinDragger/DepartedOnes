using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_ClossDialogueBox : StoryScriptPlayerAction
{
    public override string processHead => "ClossDialogueBox"; 

    /// <summary>
    /// 瞬间关闭 无需阻挡
    /// </summary>
     
    public override void ProcessParamLine(string[] paramLine)
    {
        DialogManager.instance.DeActiveBox();
    }
}
