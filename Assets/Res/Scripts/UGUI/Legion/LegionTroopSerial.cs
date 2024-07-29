using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 军团部队的战斗序列
/// </summary>
public class LegionTroopSerial : MonoBehaviour
{
    public int serialType;
    //[HideInInspector]
    public List<LegionTroopSlot> slots;
    public Text describeText;
    public Transform slotParent;
    public GameObject originSlot;
    public GameObject troopTitle;
    public List<TroopControl> troopList = new List<TroopControl>();
    int deltaFlag;
    bool isFullView;
    public void Init(params TroopControl[] troops)
    {
        if (troops == null || troops.Length == 0)
        {
            troopList.Clear();
            if (troopTitle)
            {
                troopTitle.transform.localPosition = new Vector3(0, 40.5f, 0);
                troopTitle.gameObject.SetActive(true);
            }
            if (describeText)
                describeText.text = "";
            if (slots != null)
            {
                foreach (var slot in slots)
                {
                    slot.gameObject.SetActive(false);
                }
            }
            return;
        }
        if (slots == null)
        {
            slots = new List<LegionTroopSlot>();
        }
        deltaFlag = 0;
        troopList.Clear();
        int totalNum = 0;
        for (int i = 0; i < troops.Length; i++)
        {
            troopList.Add(troops[i]);
            totalNum += troops[i].nowNum;
        }
        ArrayUtil.ListShowFit(slots, troopList, originSlot, slotParent, (slot, data) =>
        {
            slot.gameObject.SetActive(true);
            slot.Init(data);
        });
        if (!isFullView)
        {
            troopTitle.gameObject.SetActive(false);
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].transform.localPosition = new Vector3(0, 24 - 9 * i, 0);
            }
        }
        else
        {
            troopTitle.gameObject.SetActive(true);
            troopTitle.transform.localPosition = new Vector3(0, 10, 0);
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].transform.localPosition = GetTargetLocalPos(i);
            }
        }
        if (describeText)
        {
            if (troops.Length > 1)
            {
                describeText.text = $"X{troops.Length}({totalNum})";
            }
            else
            {
                describeText.text = $"{totalNum}";
            }
        }
    }

    public void DesSerialRefresh(TroopControl[] troops)
    {
        troopList.Clear();
        int totalNum = 0;
        for (int i = 0; i < troops.Length; i++)
        {
            troopList.Add(troops[i]);
            totalNum += troops[i].nowNum;
        }
        if (describeText)
        {
            if (troops.Length > 1)
            {
                describeText.text = $"X{troops.Length}({totalNum})";
            }
            else
            {
                describeText.text = $"{totalNum}";
            }
        }
        if (troops.Length == 0)
        {
            if (troopTitle)
            {
                troopTitle.transform.localPosition = new Vector3(0, 40.5f, 0);
                troopTitle.gameObject.SetActive(true);
            }
            if (describeText)
                describeText.text = "";
        }
        else
        {
            SwitchToFullShow();
        }

    }


    /// <summary>
    /// 切换到全数显示模式
    /// </summary>
    public void SwitchToFullShow()
    {
        isFullView = true;
        if (troopList.Count == 0) return;
        troopTitle.gameObject.SetActive(true);
        for (int i = 0; i < troopList.Count; i++)
        {
            slots[i].transform.DOLocalMove(GetTargetLocalPos(i), 0.2f).SetEase(Ease.OutSine);
        }
        troopTitle.transform.localPosition = new Vector3(0, 10, 0);

        //生成没有的
    }

    Vector3 GetTargetLocalPos(int flag)
    {
        return new Vector3(0, 10 - 85 * flag - 85, 0);
    }

    /// <summary>
    /// 收缩至有限显示
    /// </summary>
    public void ShrinkToLimitedShow()
    {
        isFullView = false;
        if (troopList.Count != 0)
        {
            CoroutineManager.DelayedCoroutine(0.1f, () =>
            {
                troopTitle.gameObject.SetActive(false);
            });
        }
        for (int i = 0; i < troopList.Count; i++)
        {
            Vector3 targetPos = new Vector3(0, 24 - 9 * i, 0);
            slots[i].transform.DOLocalMove(targetPos, 0.2f).SetEase(Ease.OutSine);
        }
    }


    public void RemoveLegionTroop(TroopControl troop)
    {
        //在对应UI区域去移除

    }

    /// <summary>
    /// FullShow时的双边拓展
    /// </summary>
    public void FullShowTwoSideExpend(int flag, Transform withoutObject)
    {
        float distance = 30f;
        int target = flag - 1;
        if (target >= 0 && slots.Count > target)
        {
            if (slots[target].transform != withoutObject)
                slots[target].transform.DOLocalMove(GetTargetLocalPos(target) + new Vector3(0, distance / 2, 0), 0.1f);
        }
        target = flag;
        if (target >= 0 && slots.Count > target)
        {
            if (slots[target].transform != withoutObject)
                slots[target].transform.DOLocalMove(GetTargetLocalPos(target) + new Vector3(0, distance, 0), 0.1f);
        }
        target = flag + 1;
        if (target >= 0 && slots.Count > target)
        {
            if (slots[target].transform != withoutObject)
                slots[target].transform.DOLocalMove(GetTargetLocalPos(target) - new Vector3(0, distance, 0), 0.1f);
        }
        target = flag + 2;
        if (target >= 0 && slots.Count > target)
        {
            if (slots[target].transform != withoutObject)
                slots[target].transform.DOLocalMove(GetTargetLocalPos(target) - new Vector3(0, distance / 2, 0), 0.1f);
        }
    }
    public void FullShowTwoSideShrink(int flag, Transform withoutObject)
    {
        int target = flag - 1;
        for (int i = 0; i < 4; i++)
        {
            if (target >= 0 && slots.Count > target)
            {
                if (slots[target].transform == withoutObject) return;
                slots[target].transform.DOLocalMove(GetTargetLocalPos(target), 0.1f);
            }
            target++;
        }
    }

    /// <summary>
    /// 触发点击
    /// </summary>
    public void OnClick()
    {
        var troop = troopList[deltaFlag % troopList.Count];
        ///troop.unitType
        UIManager.Instance.ShowUI("UnitDetailPanel", (ui) =>
        {
            (ui as UnitDetailPanel).InitByTroop(troop);
        });
    }

    /// <summary>
    /// 触发拖拽
    /// </summary>
    public void OnDrag(Vector2 delta)
    {
        //向下拖拽
        if (delta.y < 0 && Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
        {
            var ui = GetComponentInParent<RogueTopDetailUI>();
            if (ui != null)
            {
                ui.SwitchToFullShow();
                return;
            }
            
            GetComponentInParent<LegionTopDetailUI>().SwitchToFullShow();
        }
    }

}
