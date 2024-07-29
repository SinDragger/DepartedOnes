using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryScriptPlayerAction
{


    public virtual string processHead => "";
    /// <summary>
    /// 处理参数行
    /// </summary>
    public virtual void ProcessParamLine(string[] paramLine)
    {

    }

    public virtual bool ComfirmBlockEnd()
    {
      
        return true;
    }
}
