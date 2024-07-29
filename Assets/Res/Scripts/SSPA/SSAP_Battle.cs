using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_Battle : StoryScriptPlayerAction
{
    public override string processHead => "Battle";

    HashSet<CommandUnit> commands = new HashSet<CommandUnit>();

    public override void ProcessParamLine(string[] paramLine)
    {
        commands.Clear();
        var ids = paramLine[1].Split(',');
        Character[] characters = new Character[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            characters[i] = StoryManager.instance.GetCharacter(ids[i]);
            if (characters[i] != null)
            {
                commands.Add(characters[i].refData.commander);
            }
        }

        foreach (var com in commands)
        {
            com.TriggerCommandToAggressive();
        }
        StoryManager.instance.Deactive();
    }

}
