using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_GroupUp : StoryScriptPlayerAction
{
    public override string processHead => "GroupUp";

    public override void ProcessParamLine(string[] paramLine)
    {
        var IDs = paramLine[1].Split(',');
        Character[] characters = new Character[IDs.Length];
        for (int i = 0; i < IDs.Length; i++)
        {
            characters[i] = StoryManager.instance.GetCharacter(IDs[i]);

        }

        if (int.TryParse(paramLine[2], out int belong))
            UnitControlManager.instance.CharactersGroupUp(belong, characters);
        else
            UnitControlManager.instance.CharactersGroupUp(1, characters);
    }


}
