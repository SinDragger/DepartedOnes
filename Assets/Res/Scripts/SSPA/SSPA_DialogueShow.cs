using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_DialogueShow : StoryScriptPlayerAction
{
    string dialogueDetails;
    public override string processHead => "DialogueShow";

    public override void ProcessParamLine(string[] paramLine)
    {
        
        System.Text.StringBuilder builder = new System.Text.StringBuilder(paramLine[1]);
        for (int i = 2; i < paramLine.Length; i++)
        {
            builder.Append(" ");
            builder.Append(paramLine[i]);
        }
        dialogueDetails = builder.ToString();
        DialogManager.instance.ShowDialog(dialogueDetails);

    }
    
    public override bool ComfirmBlockEnd()
    {
        return DialogManager.instance.CompleteNowDialogue;
    }
}
