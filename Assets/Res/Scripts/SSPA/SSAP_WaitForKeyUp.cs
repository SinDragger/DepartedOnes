using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_WaitForKeyUp : StoryScriptPlayerAction
{
    public override string processHead => "WaitForKeyUp";

    /// <summary>
    /// ˲��ر� �����赲
    /// </summary>
    string key;
    public override void ProcessParamLine(string[] paramLine)
    {
        key = paramLine[1];
    }

    public override bool ComfirmBlockEnd()
    {
        if (key == null) return true;
        if (Input.GetKeyUp(key.ToLower()))
        {
            return true;
        }
        return false;
    }
}
