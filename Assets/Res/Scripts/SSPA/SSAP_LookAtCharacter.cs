using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_LookAtCharacter : StoryScriptPlayerAction
{
    public override string processHead => "LookAtCharacter";

    Character character;
    Vector3 pos;
    public override void ProcessParamLine(string[] paramLine)
    {

        character = StoryManager.instance.GetCharacter(paramLine[1]);
        pos = Vector3.zero;
        //将character的transfrom传给CameraControl
        if (character != null)
        {
            if (paramLine.Length == 2)
                CameraControl.Instance.LookAtTransfrom(character.refData.model.transform);
            else
            {
                string[] posParam = paramLine[2].Split(',');

                float.TryParse(posParam[0], out pos.x);
                float.TryParse(posParam[1], out pos.y);
                float.TryParse(posParam[2], out pos.z);
                CameraControl.Instance.LookAtV3(character.refData.model.transform.position + pos);
            }
        }
    }

    public override bool ComfirmBlockEnd()
    {

        //判断到达transfrom位置没有
        if (character != null)
        {
            return Vector3.Distance(CameraControl.Instance.transform.parent.position, character.refData.model.transform.position + pos) < 0.5f;
        }
        else return true;
    }

}
