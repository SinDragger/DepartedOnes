using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RogueMidDetailUI : UIPanel
{
    [SerializeField] private GameObject showPanel;
    [SerializeField] private Text storyInfo;
    [SerializeField] private LayoutGroup selectionGroup;

    private List<Button> selectionButtons = new List<Button>();
    //当前肉鸽节点
    private RogueNodeData currentNode;

    public void UpdateRogueStory(RogueNodeData data)
    {
        currentNode = data;
        storyInfo.text = data.storyDescribe;
        AdjustObjectListSize(selectionButtons, data.selections.Length);
        ClearEvent();
        for (int i = 0; i < data.selections.Length; ++i)
        {
            int flag = i;
            selectionButtons[i].GetComponentInChildren<Text>().text = data.selections[i].describe;
            selectionButtons[i].SetBtnEvent(() => OnSelectionButtonClick(flag));
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(selectionGroup.GetComponent<RectTransform>());
    }


    private void OnSelectionButtonClick(int index)
    {
        RogueManager.instance.ExcuteSelectionBehavior(currentNode.selections[index]);
    }

    public override void OnShow(bool withAnim = true)
    {
        base.OnShow(withAnim);
        showPanel.SetActive(true);
    }

    public override void OnHide(bool withAnim = true)
    {
        base.OnHide(withAnim);
        showPanel.SetActive(false);
    }

    private void AdjustObjectListSize(List<Button> list, int count)
    {
        if (list.Count == count) return;
        else if (list.Count > count)
        {
            while (list.Count != count)
            {
                Button b = list[list.Count - 1];
                GameObjectPoolManager.Instance.Recycle(b.gameObject, "Prefab/" + Constant_RogueData.SelectionUIButton);
                list.RemoveAt(list.Count - 1);
            }
        }
        else if (list.Count < count)
        {
            while (list.Count != count)
            {
                Button b = GameObjectPoolManager.Instance.Spawn("Prefab/" + Constant_RogueData.SelectionUIButton).GetComponent<Button>();
                b.transform.SetParent(selectionGroup.transform);
                b.GetComponent<RectTransform>().Reset();
                list.Add(b);
            }
        }
    }

    private void ClearEvent()
    {
        foreach (var button in selectionButtons)
            button.onClick.RemoveAllListeners();
    }
}
