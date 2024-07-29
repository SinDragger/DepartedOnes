using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_WaitForKeyDown : StoryScriptPlayerAction
{
    public override string processHead => "WaitForKeyDown";

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
        if (Input.GetKeyDown(key.ToLower()))
        {
            return true;
        }
        return false;
    }

}
