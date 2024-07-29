using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEntity
{
    public QuestData data { get; private set; }
    public int questCount { get; private set; }

    public bool isComplete;

    public QuestEntity(QuestData questData)
    {
        data = questData;
        questCount = 0;
    }

    public void OnEventInvoke(EventData eventData)
    {
        if (isComplete) return;
        int count = data.eventResolvers[0].ResolveData(eventData);
        if (count > 0)
        {
            questCount += count;
            QuestManager.Instance.UpdateUI(data.idName);
            if (questCount >= data.questMaxCount)
            {
                isComplete = true;
                //任务完成
                foreach (var action in data.eventActions)
                    action.OnInvoke(eventData);
            }
        }
    }

    public void RegisterEvent()
    {
        foreach (var dialog in data.dialogs)
        {
            if (dialog.contextIdName.StartsWith("Start"))
            {
                EventManager.Instance.ProcessMapEvent(dialog);
                break;
            }
        }
        EventManager.Instance.RegistEvent(data.eventResolvers[0].eventName, OnEventInvoke);
    }

    public void UnregisterEvent()
    {
        foreach (var dialog in data.dialogs)
        {
            if (dialog.contextIdName.StartsWith("End"))
            {
                EventManager.Instance.ProcessMapEvent(dialog);
                break;
            }
        }
        EventManager.Instance.UnRegistEvent(data.eventResolvers[0].eventName, OnEventInvoke);
    }
}
