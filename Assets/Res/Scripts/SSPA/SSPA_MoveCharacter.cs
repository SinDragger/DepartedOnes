using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_MoveCharacter : StoryScriptPlayerAction
{
    public override string processHead => "MoveCharacter";

    /// <summary>
    /// 控制角色移动
    /// </summary>
    /// <param name="paramLine">
    /// 0:方法名字
    /// 1:目标角色的IdName
    /// 2:Pos
    /// </param>
    public override void ProcessParamLine(string[] paramLine)
    {
        //数组的长度至少为4
        //通过传入的ID 和v3让对应ID的角色移动到v3的位置
        var character = StoryManager.instance.GetCharacter(paramLine[1]);


        string[] posParam = paramLine[2].Split(',');
        Vector3 pos = Vector3.zero;
        float.TryParse(posParam[0], out pos.x);
        float.TryParse(posParam[1], out pos.y);
        float.TryParse(posParam[2], out pos.z);
     

        character.refData.model.actionModel.SetPositionMoveTo(pos);
    }
  

    
}
