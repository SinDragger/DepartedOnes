using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegionTopDetailUI : UIPanel
{
    public override string uiPanelName => "LegionTopDetailUI";
    public GameObject showPanel;
    public GameObject generalIcon;
    public GameObject displayArea;
    public GameObject dark;
    public List<LegionTroopSerial> troopSerials = new List<LegionTroopSerial>();
    public Image headIcon;
    public GameObject generalHPBar;
    public Image generalHPSlide;
    [SerializeField]
    Button resourceMapButton;
    [SerializeField]
    DetailResourceMap resourceMap;
    public Text movementText;
    public Text resText;
    LegionControl nowLegion;
    //TODO:增加Select选取与指向
    public RecruitArea area;
    //增加详情列表的滚动与点击展开
    //对列进行可滚动化
    public Button generalRecoverButton;

    private void Start()
    {
        EventManager.Instance.RegistEvent(MapEventType.LEGION_SELECT, (e) => InitByLegionData(e.GetValue<LegionControl>("Legion")));
        resourceMapButton.SetBtnEvent(() =>
        {
            resourceMap.gameObject.SetActive(!resourceMap.gameObject.activeSelf);
        });
        LegionManager.Instance.onLegionElininated += LegionRemoved;
        generalRecoverButton.SetBtnEvent(() =>
        {
            if (nowLegion != null && nowLegion.mainGeneral != null)
            {
                int delta = (int)Mathf.Ceil(nowLegion.mainGeneral.LifeLostPercent);
                int number = GameManager.instance.playerData.soulPoint;
                if (number > delta)
                {
                    GameManager.instance.playerData.soulPoint -= delta;
                    nowLegion.mainGeneral.life = nowLegion.mainGeneral.maxLife;
                }
            }
        });
    }

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
            area.Hide();
        }
        UpdateUI(true);
    }

    void UpdateUI(bool fullUpdate = false)
    {
        if (nowLegion == null) return;
        //区块的数据变动
        //对其数据进行获取
        resourceMap.Init(nowLegion.resourcePool.resourceCarry);
        resourceMap.InitEquipStore(nowLegion.equipsTotalNumber);
        if (nowLegion.mainGeneral != null)
        {
            headIcon.sprite = nowLegion.mainGeneral.GetHeadIcon();
            headIcon.gameObject.SetActive(true);
            generalHPBar.gameObject.SetActive(true);
            generalHPSlide.fillAmount = (float)nowLegion.mainGeneral.life / (float)nowLegion.mainGeneral.maxLife;
        }
        else
        {
            headIcon.gameObject.SetActive(false);
            generalHPBar.gameObject.SetActive(false);
        }
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(displayArea.transform.parent as RectTransform);
        //if (nowLegion.mainGeneral != null && nowLegion.mainGeneral.life < nowLegion.mainGeneral.maxLife)
        //{
        //    generalRecoverButton.gameObject.SetActive(true);
        //}
        //else
        //{
        //    generalRecoverButton.gameObject.SetActive(false);
        //}
        if (nowLegion.unitChanged)
        {
            for (int i = 0; i < troopSerials.Count - 1; i++)
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
            nowLegion.unitChanged = false;
            return;
        }
        //完整的数据变动
        if (!nowLegion.uiDataChanged && !fullUpdate) return;
        nowLegion.uiDataChanged = false;
        movementText.text = nowLegion.moveSpeed.ToString("F1");
        resText.text = nowLegion.ResCarryNum.ToString();
        bool activeStack = nowLegion.stackTroops.Count != 0;
        //int numCount = activeStack ? nowShowLegion.stackTroops.Count : nowShowLegion.troops.Count;
        //聚合与同类合并
        for (int i = 0; i < troopSerials.Count - 1; i++)
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
        if (nowLegion.troopEquips != null)
        {
            troopSerials[troopSerials.Count - 1].Init(nowLegion.troopEquips.ToArray());
        }
        else
        {
            troopSerials[troopSerials.Count - 1].Init(null);
        }
    }

    public void TriggerRecover()
    {

    }

    /// <summary>
    /// 改成移除
    /// </summary>
    /// <param name="legion"></param>
    public void LegionRemoved(LegionControl legion)
    {
        //当前显示的部队被歼灭了
        if (nowLegion == legion)
        {
            OnHide(false);
        }
    }

    public override void OnShow(bool withAnim = true)
    {
        showPanel.SetActive(true);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        showPanel.SetActive(false);
        area.Hide();
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
                if (lastTargetFlagY >= 0 && lastTargetFlagY < troopSerials[lastTargetFlagX].troopList.Count)
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
                if (targetFlag >= 0 && targetFlag < troopSerials[lastTargetFlagX].troopList.Count)
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
        int targetX = -1;
        int targetY = -1;
        var slotPos = CameraControl.Instance.mainCamera.WorldToScreenPoint(slot.transform.position);
        for (int i = 0; i < troopSerials.Count - 1; i++)
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

    public void OnRecruitClick(int index)
    {
        if (area.gameObject.activeSelf)
        {
            area.Hide();
            return;
        }
        if (nowLegion.belong != GameManager.instance.belong) return;
        //dark.gameObject.SetActive(true);
        //SwitchToFullShow();
        UnitData unitData = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>("DriedSkeleton");
        UIManager.Instance.ShowUI("RaiseUnitPanel", (ui) =>
        {
            (ui as RaiseUnitPanel).Init(nowLegion, unitData, () =>
            {
                //dark.gameObject.SetActive(false);
            });
        });
        //area.gameObject.SetActive(true);
        //area.Show();
    }
}
