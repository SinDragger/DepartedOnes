using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueNodeSelectionBehavior_ResControl : RogueNodeSelectionBehavior
{
    public string resName;
    public int amount;

    public override void Excute()
    {
        RogueManager.instance.ResourceChange(resName, amount);
    }
}
