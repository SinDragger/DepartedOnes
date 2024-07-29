using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RogueTopDetailUI : UIPanel
{
    public override string uiPanelName => Constant_UIPanel.UIPanel_RoguePanel;
    public GameObject showPanel;
    public GameObject generalIcon;
    public GameObject displayArea;
    public GameObject dark;
    public List<LegionTroopSerial> troopSerials = new List<LegionTroopSerial>();
    public Image headIcon;
    LegionControl nowLegion;
    //TODO:增加Select选取与指向
    //增加详情列表的滚动与点击展开
    //对列进行可滚动化

    [SerializeField] private Slider suppliesSlider;
    [SerializeField] private Text suppliesNum;
    [SerializeField] private Text fatigueNum;

    public void InitByLegionData(LegionControl legion)
    {
        if (legion == null)
        {
            showPanel.SetActive(false);
            return;
        }
        if (legion != nowLegion)
        {
            nowLegion = legion;
        }
        //showPanel.SetActive(nowShowLegion != null);
        UpdateUI(true);
    }

    void UpdateUI(bool fullUpdate = false)
    {
        if (nowLegion == null) return;
        if (!nowLegion.uiDataChanged && !fullUpdate) return;
        nowLegion.uiDataChanged = false;
        bool activeStack = nowLegion.stackTroops.Count != 0;
        //int numCount = activeStack ? nowShowLegion.stackTroops.Count : nowShowLegion.troops.Count;
        //聚合与同类合并


        for (int i = 0; i < troopSerials.Count; i++)
        {
            if (activeStack)
            {
                if (nowLegion.stackTroops.ContainsKey(i))
                {
                    troopSerials[i].Init(nowLegion.stackTroops[i].ToArray());
                }
                else
                {
                    troopSerials[i].Init(null);
                }
            }
            else
            {
                troopSerials[i].Init(nowLegion.troops[i]);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(displayArea.transform.parent as RectTransform);

        if (nowLegion.mainGeneral != null)
        {
            headIcon.sprite = nowLegion.mainGeneral.GetHeadIcon();
            headIcon.gameObject.SetActive(true);
        }
        else
        {
            headIcon.gameObject.SetActive(false);
        }

    }

    [Button]
    public void UpdateResUI(int fatigue, int supplies)
    {
        fatigueNum.text = fatigue.ToString();
        int st = supplies / 100;
        DOTween.To(() => suppliesSlider.value, i => suppliesSlider.value = i, st, 0.5f);
        suppliesNum.text = supplies.ToString();
    }


    public override void OnShow(bool withAnim = true)
    {
        showPanel.SetActive(true);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        showPanel.SetActive(false);
        base.OnHide(withAnim);
    }

    public void SwitchToFullShow()
    {
        dark.gameObject.SetActive(true);

        for (int i = 0; i < troopSerials.Count; i++)
        {
            Vector3 fixlocalPos = troopSerials[i].transform.localPosition;
            troopSerials[i].SwitchToFullShow();
            foreach (var slot in troopSerials[i].slots)
            {
                if (GameManager.instance.belong != nowLegion.belong) continue;
                var dragAbleObject = slot.GetComponent<DragAbleObject>();
                if (i != troopSerials.Count - 1)
                    dragAbleObject.dragAble = true;
                dragAbleObject.onDragStart.RemoveAllListeners();
                Canvas canvas = null;
                dragAbleObject.onDragStart.AddListener(() =>
                {
                    canvas = dragAbleObject.gameObject.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 1000;
                });


                dragAbleObject.onDraging.RemoveAllListeners();
                dragAbleObject.onDraging.AddListener((localPos) =>
                {
                    CheckTargetPos(slot.gameObject);//- (Vector2)fixlocalPos
                });

                dragAbleObject.onDragEnd.RemoveAllListeners();
                dragAbleObject.onDragEnd.AddListener(() =>
                {
                    Destroy(canvas);
                    DragOver(slot, dragAbleObject);
                });
            }
        }
    }

    void DragOver(LegionTroopSlot legionTroopSlot, DragAbleObject dragAbleObject)
    {
        //Debug.LogError($"{lastTargetFlagX} {lastTargetFlagY}");
        if (lastTargetFlagX >= 0)
        {
            //格子的移动插入
            int from = nowLegion.MoveTroop(legionTroopSlot.troopData, lastTargetFlagX, lastTargetFlagY);
            if (from != lastTargetFlagX)
            {
                troopSerials[from].slots.Remove(legionTroopSlot);
                troopSerials[from].DesSerialRefresh(nowLegion.stackTroops[from].ToArray());
                if (lastTargetFlagY < troopSerials[lastTargetFlagX].troopList.Count)
                {
                    troopSerials[lastTargetFlagX].slots.Insert(lastTargetFlagY, legionTroopSlot);
                }
                else
                    troopSerials[lastTargetFlagX].slots.Insert(troopSerials[lastTargetFlagX].troopList.Count, legionTroopSlot);
            }
            else
            {
                int preFlag = troopSerials[from].slots.IndexOf(legionTroopSlot);
                troopSerials[from].slots.Remove(legionTroopSlot);
                int targetFlag = lastTargetFlagY;
                bool minus = preFlag < lastTargetFlagY;
                if (preFlag < lastTargetFlagY)
                {
                    targetFlag--;
                }
                if (targetFlag < troopSerials[lastTargetFlagX].troopList.Count)
                {
                    troopSerials[lastTargetFlagX].slots.Insert(targetFlag, legionTroopSlot);
                }
                else
                    troopSerials[lastTargetFlagX].slots.Insert((minus ? -1 : 0) + troopSerials[lastTargetFlagX].troopList.Count, legionTroopSlot);

            }
            legionTroopSlot.transform.parent = troopSerials[lastTargetFlagX].slotParent;
            foreach (var slot in troopSerials[lastTargetFlagX].slots)
            {
                slot.transform.SetAsLastSibling();
            }
            troopSerials[lastTargetFlagX].DesSerialRefresh(nowLegion.stackTroops[lastTargetFlagX].ToArray());
            //刷新UI——单位嵌入
            //优先刷新
        }
        else
        {
            dragAbleObject.transform.localPosition = dragAbleObject.startDragPos;
        }
    }

    const float checkX = 92f;
    const float checkY = 84f;

    int lastTargetFlagX = -1;
    int lastTargetFlagY = -1;
    public void CheckTargetPos(GameObject slot)
    {
        Debug.LogError("Checked");
        int targetX = -1;
        int targetY = -1;
        var slotPos = CameraControl.Instance.mainCamera.WorldToScreenPoint(slot.transform.position);
        for (int i = 0; i < troopSerials.Count; i++)
        {
            var serialPos = CameraControl.Instance.mainCamera.WorldToScreenPoint(troopSerials[i].transform.position);
            var deltaPos = slotPos - serialPos;
            if (Mathf.Abs(deltaPos.x) < checkX)
            {
                targetX = i;
                targetY = (int)(-(deltaPos.y - 10f) / checkY);
            }
            //Debug.LogError($"Serial{i}_local {slotPos - serialPos}");
        }

        if (targetX != lastTargetFlagX || targetY != lastTargetFlagY)
        {
            if (lastTargetFlagX >= 0)
            {
                troopSerials[lastTargetFlagX].FullShowTwoSideShrink(lastTargetFlagY - 1, slot.transform);
            }
            //TODO:取消移动
            lastTargetFlagX = targetX;
            lastTargetFlagY = targetY;
            //取消之前的锁定延展
            if (lastTargetFlagX >= 0)
                troopSerials[lastTargetFlagX].FullShowTwoSideExpend(lastTargetFlagY - 1, slot.transform);
        }

    }


    public void ShrinkToLimitedShow()
    {
        CoroutineManager.DelayedCoroutine(0.1f, () =>
        {
            dark.gameObject.SetActive(false);
        });
        for (int i = 0; i < troopSerials.Count; i++)
        {
            troopSerials[i].ShrinkToLimitedShow();
            foreach (var slot in troopSerials[i].slots)
            {
                slot.GetComponent<DragAbleObject>().dragAble = false;
            }
        }
    }


    //UI上进行拖动与拖拽

    private void Update()
    {
        //临时方案 每帧刷新
        UpdateUI();
    }
}
