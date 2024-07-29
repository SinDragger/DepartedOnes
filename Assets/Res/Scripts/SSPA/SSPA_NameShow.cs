using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_NameShow : StoryScriptPlayerAction
{
    public override string processHead => "NameShow";

    /// <summary>
    /// 显示Dialog的Name
    /// </summary>
    /// <param name="paramLine">
    /// 1:说话的角色ID
    /// 2.:说话的角色Name
    /// </param>

    public override void ProcessParamLine(string[] paramLine)
    {
        var idAndName = paramLine[1].Split(',');

        //第一步 根据ID找到对应的对话角色GB
        var character = StoryManager.instance.GetCharacter(idAndName[0]);
        Vector3 pos = character.refData.model.gameObject.transform.position;
        DialogManager.instance.SetBoxTransFrom(pos);

        //第二步 设置对话框名字
        DialogManager.instance.SetBoxName(idAndName[1]);
    }
}
