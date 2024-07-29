using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestPanelUI : UIPanel
{
    [SerializeField] private LayoutGroup layoutGroup;
    
    private Dictionary<string, QuestDetailUI> detailUISet = new Dictionary<string, QuestDetailUI>();

    public QuestDetailUI GenDetailUI(QuestEntity entity)
    {
        QuestDetailUI ui = GameObjectPoolManager.Instance.Spawn("Prefab/QuestDetail").GetComponent<QuestDetailUI>();
        ui.transform.SetParent(layoutGroup.transform);
        ui.GetComponent<RectTransform>().Reset();
        detailUISet.Add(entity.data.idName, ui);
        ui.UpdateQuestDetail(entity);
        return ui;
    }

    public void RecycleDetailUI(QuestEntity entity)
    {
        if (detailUISet.ContainsKey(entity.data.idName))
        {
            var ui = detailUISet[entity.data.idName];
            detailUISet.Remove(entity.data.idName);
            GameObjectPoolManager.Instance.Recycle(ui.gameObject, "Prefab/QuestDetail");
        }
        else
        {
            Debug.LogError("cant find detailui whith id:" + entity.data.idName);
        }
        
    }

    public void UpdateUIAll(List<QuestEntity> entities)
    {
        foreach (var entity in entities)
        {
            //if (detailUISet.ContainsKey(entity.data.idName))
            {
                detailUISet[entity.data.idName].UpdateQuestDetail(entity);
            }
        }
    }

    public void UpdateUI(QuestEntity entity)
    {
        //if (detailUISet.ContainsKey(entity.data.idName))
        {
            detailUISet[entity.data.idName].UpdateQuestDetail(entity);
        }
    }
}
