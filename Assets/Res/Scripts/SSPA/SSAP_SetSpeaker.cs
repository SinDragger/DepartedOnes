using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_SetSpeaker : StoryScriptPlayerAction
{
    public override string processHead => "SetSpeaker";
    public float yOffset = 100;
    /// <summary>
    /// 0 SetSpeaker
    /// 1 说话的角色ID
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {

        //TODO 需要向CharacterManager 中获取到paramLine[1]的对应ID的GameObject的position
        Vector3 focusPosition = Camera.main.WorldToScreenPoint(ARPGManager.Instance.currentGeneralControl.transform.position) - new Vector3(Screen.width / 2, Screen.height / 2 - yOffset, 0);
        focusPosition.z = 0;

        DialogManager.instance.SetBoxTransFrom(focusPosition);


    }

}
