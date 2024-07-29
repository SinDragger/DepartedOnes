using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_CheckCharacter : StoryScriptPlayerAction
{
    public override string processHead => "CheckCharacter";

    /// <summary>
    /// 
    /// 0:CheckCharacter
    /// 1:ID
    /// 2:初始化位置
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {
        var character = StoryManager.instance.GetCharacter(paramLine[1]);
        //TODO 通过数组第一个元素找到gameObject->character
        string[] posParam = paramLine[2].Split(',');
        Vector3 pos = Vector3.zero;
        float.TryParse(posParam[0], out pos.x);
        float.TryParse(posParam[1], out pos.y);
        float.TryParse(posParam[2], out pos.z);
        character.refData.model.gameObject.transform.position = pos;
        if (paramLine[3].Equals("Right"))
        {
            
            character.refData.model.FaceToRight();
        }
        else
        {
            if (character.refData.model.actionModel.IsRight)
                character.refData.model.FaceToLeft();
            //character.refData.model.actionModel.ShiftModelFaceTo(false);
        }
       
    }

    public override bool ComfirmBlockEnd()
    {
        return base.ComfirmBlockEnd();
    }

}
