using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_ConstructionChange : EventTriggerData
{
    public string targetConstructionIdName;
    public override bool Process()
    {
        SectorConstruction construction = context["Construction"] as SectorConstruction;
        construction.ConstructionTransfer(targetConstructionIdName);
        return true;
    }
}
