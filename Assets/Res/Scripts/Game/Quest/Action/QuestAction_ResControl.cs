using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAction_ResControl : QuestEventAction
{
    public string[] resIds;
    public string[] resAmount;

    public override void OnInvoke(EventData eventData)
    {
        for (int i = 0; i < resIds.Length; i++)
        {
            GameManager.instance.playerForce.resourcePool.ChangeResource(resIds[i], int.Parse(resAmount[i]));
        }
    }
}
