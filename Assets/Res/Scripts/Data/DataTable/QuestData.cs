using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : AggregationEntity
{
    public string questName;
    public string questDescribe;
    public int questMaxCount;

    public Event_ShowDialog[] dialogs;

    public QuestEventResolver[] eventResolvers;
    public QuestEventAction[] eventActions;
}
