using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_Wait : StoryScriptPlayerAction
{
    float waitTime;
    float currentTime;
    public override string processHead => "Wait";


    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {
        currentTime = 0;

        if (!float.TryParse(paramLine[1], out  waitTime))
        {
            // 如果成功解析字符串为浮点数，floatValue 包含了转换后的值
            waitTime = 0;
        }


    }
    public override bool ComfirmBlockEnd()
    {
        currentTime += Time.deltaTime;
        return currentTime >= waitTime;
    }

}
