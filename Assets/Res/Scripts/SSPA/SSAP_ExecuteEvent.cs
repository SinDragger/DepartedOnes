using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_ExecuteEvent : StoryScriptPlayerAction
{
    public override string processHead => "ExecuteEvent";

    public override void ProcessParamLine(string[] paramLine)
    {
        var array = paramLine[1].Split(',');
        foreach (var item in array)
        {
            EventManager.Instance.DispatchEvent(new EventData(item));
        }

    }
}
