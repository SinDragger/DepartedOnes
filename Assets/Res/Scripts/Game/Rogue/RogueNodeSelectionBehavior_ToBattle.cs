using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueNodeSelectionBehavior_ToBattle : RogueNodeSelectionBehavior
{
    public string battleScenceId;

    public override void Excute()
    {
        RogueManager.instance.EnterRogueBattle(battleScenceId);
    }
}
