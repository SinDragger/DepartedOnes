using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAction_PrefChange : QuestEventAction
{
    public string prefName;

    public override void OnInvoke(EventData eventData)
    {
        PlayerPrefs.SetInt(prefName, 1);
    }
}
