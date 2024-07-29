using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailUI : MonoBehaviour
{
    public Text questName;
    public Text questDescription;

    private QuestEntity entity;

    public void UpdateQuestDetail(QuestEntity _entity)
    { 
        entity = _entity;
        questName.text = entity.data.questName + " ( " + entity.questCount + " / " + entity.data.questMaxCount + " )";
        questDescription.text = entity.data.questDescribe;
    }


}
