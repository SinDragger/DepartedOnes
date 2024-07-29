using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_MonitoringBattle : StoryScriptPlayerAction
{
    public override string processHead => "MonitoringBattle";
    const float wiatTime = 1f;
    float timmer;
    public override void ProcessParamLine(string[] paramLine)
    {
        timmer = 0;
    }
    public override bool ComfirmBlockEnd()
    {

        if (UnitControlManager.instance.IsEnermyAllDie())
        {
            timmer += Time.deltaTime;
            if (timmer > wiatTime)
            {
                Debug.Log("你赢了！！！");
                StoryManager.instance.Active();
                return true;

            }
            else return false;
        }
        else if (UnitControlManager.instance.IsPlayerAllDie())
        {
            Debug.LogError("剧情战斗死亡如何做处理");
            //  OnGameFail();
        }
        return false;
    }
}
