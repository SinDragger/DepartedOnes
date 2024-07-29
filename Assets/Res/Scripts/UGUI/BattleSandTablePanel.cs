using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSandTablePanel : UIPanel
{
    public override string uiPanelName => "BattleSandTablePanel";
    public GameObject root;
    public Text distanceText;
    public WarBattle warBattle;
    public Transform[] lines;
    public List<List<LegionTroopAutoSlot>> troopsIcon;
    public Transform left;
    public Transform right;
    public Transform distanceArea;
    public GameObject prefab;
    public void Init(WarBattle warBattle)
    {
        this.warBattle = warBattle;
        if (troopsIcon == null)
        {
            troopsIcon = new List<List<LegionTroopAutoSlot>>(8);
            for (int i = 0; i < 8; i++)
            {
                troopsIcon.Add(new List<LegionTroopAutoSlot>(8));
            }
        }
        for (int i = 0; i < warBattle.battleTroopsList.Count; i++)
        {
            InitLine(lines[i], troopsIcon[i], warBattle.battleTroopsList[i], i < 4);
        }
        if (warBattle != null)
        {
            battleTime = warBattle.behaviourChain.nowTime;
            int distance = Mathf.CeilToInt(warBattle.nowDistance);
            if (distance > 0)
            {
                distanceText.text = $"{distance}";
                left.transform.localPosition = new Vector3(-400f, -30f, 0);
                right.transform.localPosition = new Vector3(400f, -30f, 0);
                distanceArea.localScale = Vector3.one;
                distanceArea.GetComponent<CanvasGroup>().alpha = 1f;
                hasClosed = false;
            }
            else
            {
                left.transform.localPosition = new Vector3(-330f, -30f, 0);
                right.transform.localPosition = new Vector3(330f, -30f, 0);
                distanceArea.localScale = Vector3.zero;
                distanceArea.GetComponent<CanvasGroup>().alpha = 0f;
                hasClosed = true;
            }
        }
        warBattle.troopMoved += OnTroopMoved;
    }

    void OnTroopMoved(AutoBattleTroop troop, int fromLine, int toLine)
    {
        var autoSlot = troopsIcon[fromLine].Find((slot) => slot.autoTroop == troop);
        troopsIcon[fromLine].Remove(autoSlot);
        troopsIcon[toLine].Add(autoSlot);
        autoSlot.transform.SetParent(lines[toLine]);
        UpdateSlots(fromLine);
        UpdateSlots(toLine);
    }
    //面板移动330为结束
    public void InitLine(Transform parent, List<LegionTroopAutoSlot> slotlist, List<AutoBattleTroop> troops, bool isRight)
    {
        int max = troops.Count;
        float delta = GetDelta(max);
        int flag = 0;
        ArrayUtil.ListShowFit(slotlist, troops, prefab, parent, (slot, troop) =>
        {
            slot.autoTroop = troop;
            slot.gameObject.SetActive(true);
            slot.Init(troop.originTroop);
            slot.transform.localPosition = Vector3.zero + new Vector3(0, flag * 70 - delta + 35f, 0);
            slot.SetFaceTo(isRight);
            flag++;
        });
    }

    float GetDelta(int max)
    {
        float delta = 0f;
        if (max % 2 == 1) delta += 35f;
        delta += (max / 2) * 70;
        return delta;
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    public override void OnShow(bool withAnim = true)
    {
        //防止直接高速化进入
        TimeManager.Instance.SetTempMaxSpeedProtect();
        //TimeManager.Instance.SwitchTimeSpeed();
        root.SetActive(true);
    }

    /// <summary>
    /// 界面隐藏
    /// </summary>
    public override void OnHide(bool withAnim = true)
    {
        root.SetActive(false);
        warBattle.troopMoved -= OnTroopMoved;
        warBattle = null;
        //全清除重置
        for (int i = 0; i < troopsIcon.Count; i++)
        {
            for (int j = 0; j < troopsIcon[i].Count; j++)
            {
                Destroy(troopsIcon[i][j].gameObject);
            }
            troopsIcon[i].Clear();
        }
    }

    bool hasClosed;
    float battleTime;
    private void LateUpdate()
    {
        if (warBattle != null)
        {
            battleTime = warBattle.behaviourChain.nowTime;
            int distance = Mathf.CeilToInt(warBattle.nowDistance);
            distanceText.text = $"{distance}";
            if (distance <= 0 && !hasClosed)
            {
                left.DOLocalMove(new Vector3(-330f, -30, 0), 0.2f).SetEase(Ease.InSine);
                right.DOLocalMove(new Vector3(330f, -30, 0), 0.2f).SetEase(Ease.InSine);
                distanceArea.DOScale(Vector3.zero, 0.2f);
                distanceArea.GetComponent<CanvasGroup>().DOFade(0f, 0.1f).SetEase(Ease.InSine);
                hasClosed = true;
            }
            UpdateSlots();
            if (warBattle.winnerBelongTo != -1)
            {
                OnHide();
            }
        }
    }
    List<LegionTroopAutoSlot> needRemoveSlot = new List<LegionTroopAutoSlot>();
    void UpdateSlots()
    {
        LegionTroopAutoSlot nowSlot;
        for (int i = 0; i < troopsIcon.Count; i++)
        {
            for (int j = 0; j < troopsIcon[i].Count; j++)
            {
                nowSlot = troopsIcon[i][j];
                nowSlot.PosAnimUpdate(battleTime);
                if (nowSlot.autoTroop.aliveNumber <= 0)
                {
                    needRemoveSlot.Add(nowSlot);
                }
            }
            if (needRemoveSlot.Count > 0)
            {
                for (int j = 0; j < needRemoveSlot.Count; j++)
                {
                    needRemoveSlot[j].gameObject.SetActive(false);
                    troopsIcon[i].Remove(needRemoveSlot[j]);
                }
                float delta = GetDelta(troopsIcon[i].Count);
                //排序刷新
                for (int j = 0; j < troopsIcon[i].Count; j++)
                {
                    nowSlot = troopsIcon[i][j];
                    nowSlot.transform.localPosition = Vector3.zero + new Vector3(0, j * 70 - delta + 35f, 0);
                }
                needRemoveSlot.Clear();
            }
        }
    }

    void UpdateSlots(int lineFlag)
    {
        LegionTroopAutoSlot nowSlot;
        for (int j = 0; j < troopsIcon[lineFlag].Count; j++)
        {
            nowSlot = troopsIcon[lineFlag][j];
            nowSlot.PosAnimUpdate(battleTime);
            if (nowSlot.autoTroop.aliveNumber <= 0)
            {
                needRemoveSlot.Add(nowSlot);
            }
        }
        if (needRemoveSlot.Count > 0)
        {
            for (int j = 0; j < needRemoveSlot.Count; j++)
            {
                needRemoveSlot[j].gameObject.SetActive(false);
                troopsIcon[lineFlag].Remove(needRemoveSlot[j]);
            }
            float delta = GetDelta(troopsIcon[lineFlag].Count);
            //排序刷新
            for (int j = 0; j < troopsIcon[lineFlag].Count; j++)
            {
                nowSlot = troopsIcon[lineFlag][j];
                nowSlot.transform.localPosition = Vector3.zero + new Vector3(0, j * 70 - delta + 35f, 0);
            }
            needRemoveSlot.Clear();
        }
    }
    
    public void ActiveBattleSwitch()
    {
        LegionManager.Instance.autoWarBattleList.Remove(warBattle);
        SceneManager.Instance.TriggerToWarBattle(warBattle);
        //时间冻结 轴封存 自动战斗移除
        //异步加载
    }
}
