using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTriggerAll : CommandTrigger
{
    public override void Trigger(CommandUnit command)
    {
        UnitControlManager.instance.ForEachTargetBelong(command.belong, (targetCommand) =>
        {
            targetCommand.TriggerCommandToAggressive();
        });
    }
}
