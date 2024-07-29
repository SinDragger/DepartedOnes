using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : Singleton<QuestManager>
{
    private List<QuestData> questDatas;

    private QuestPanelUI questPanelUI;
    private List<QuestEntity> questEnities = new List<QuestEntity>();


    protected override void Init()
    {
        base.Init();
        questDatas = DataBaseManager.Instance.GetTargetDataList<QuestData>();

        questPanelUI = UIManager.Instance.ShowUI("QuestUI").GetComponentInChildren<QuestPanelUI>();
        questPanelUI.OnHide();
    }


    public void CheckCompleteQuest()
    {
        List<QuestEntity> completeQuests = new List<QuestEntity>();
        foreach (var quest in questEnities)
        {
            if(quest.isComplete)
                completeQuests.Add(quest);
        }

        foreach (var quest in completeQuests)
        {
            UnregisterQuest(quest.data.idName);
            questEnities.Remove(quest);
        }
    }


    //战斗开始和结束时的操作
    public void ShowQuestUI()
    { 
        if(questEnities.Count > 0)
            questPanelUI.OnShow();
    }

    public void HideQuestUI()
    {
        questPanelUI.OnHide();
    }

    public void UpdateUI(string questId)
    {
        foreach (var e in questEnities)
        {
            if (e.data.idName == questId)
            {
                questPanelUI.UpdateUI(e);
                break;
            }
        }
    }

    public void RegisterQuest(string questId)
    {

        foreach (var e in questEnities)
        {
            if (e.data.idName == questId)
            {
                Debug.LogError("register quest already have a entity");
                return;
            }
        }
        Debug.LogError(questId);
        QuestData data = questDatas.Find(i => i.idName == questId);
        RegisterQuest(data);
        if (questEnities.Count > 0)
            questPanelUI.OnShow();
    }

    private void RegisterQuest(QuestData questData)
    {
        QuestEntity entity = new QuestEntity(questData);
        questPanelUI.GenDetailUI(entity);

        entity.RegisterEvent();
        questEnities.Add(entity);
    }


    public void UnregisterQuest(string questId)
    {
        QuestEntity entity = null;
        foreach (var data in questEnities)
        {
            if (data.data.idName == questId)
            {
                entity = data;
                break;
            }
        }
        if (entity != null)
        {
            questEnities.Remove(entity);
            entity.UnregisterEvent();
            questPanelUI.RecycleDetailUI(entity);
            ObjectPoolManager.Instance.Recycle(entity);
        }
        else
        {
            Debug.LogError("cant find unregister quest");
        }
        if (questEnities.Count <= 0)
            questPanelUI.OnHide();
    }
}