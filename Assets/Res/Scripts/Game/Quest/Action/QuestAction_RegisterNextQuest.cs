using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAction_RegisterNextQuest : QuestEventAction
{
    public string questIdName;

    public override void OnInvoke(EventData eventData)
    {
        CoroutineManager.StartFrameDelayedCoroutine(() =>
        {
            QuestManager.Instance.RegisterQuest(questIdName);
        });
    }
}
