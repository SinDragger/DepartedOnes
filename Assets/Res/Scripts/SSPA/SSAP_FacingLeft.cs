using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_FacingLeft : StoryScriptPlayerAction
{
    public override string processHead => "FacingLeft";

    /// <summary>
    /// 
    /// 0:CheckCharacter
    /// 1:ID
    /// 2:初始化位置
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {
        var characters = paramLine[1].Split(',');
        foreach (var c in characters)
        {
            var character = StoryManager.instance.GetCharacter(c);

            character.refData.model.FaceToLeft();
        }

    }
}
