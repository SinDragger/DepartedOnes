using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 交互选择轮，分上下级选项
/// </summary>
public class ChooseWheel : MonoBehaviour
{
    /// <summary>
    /// 测试用
    /// </summary>
    public GameObject prefab;
    public List<ItemSlot> subItems;
    public float splitAreaLength;
    public float upMaxDegree;
    public float downMaxDegree;
    /// <summary>
    /// 最大的间隔——为了紧密排列
    /// </summary>
    public float maxDegreeDelta;
    //下半是功能按键，上半是建造按键与多级菜单

    public void Init(string[] operateCast, Func<string, Sprite> iconFrom)
    {
        int length = operateCast.Length;
        if (length > 0)
        {
            float delta = upMaxDegree * 2 / length;
            if (delta > maxDegreeDelta)
            {
                delta = maxDegreeDelta;
            }
            //合计跨越的角度
            float totalCost = delta * (length - 1);
            //
            float start = 0 - totalCost / 2;
            //ArrayUtil.ListShowFit(subItems,)
            for (int i = subItems.Count; i < length; i++)
            {
                CreateSubBtn();
            }
            for (int i = 0; i < length; i++)
            {
                InitSubBtn(subItems[i], start + i * delta, operateCast[i], iconFrom);
            }
        }
    }

    public void CreateSubBtn()
    {
        var g = Instantiate(prefab, transform.position, transform.rotation, transform);
        g.gameObject.SetActive(true);
        subItems.Add(g.GetComponent<ItemSlot>());
    }

    public void InitSubBtn(ItemSlot button, float degree, string idName, Func<string, Sprite> iconFrom)
    {
        Debug.LogError("Wheel " + idName);
        var spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(idName);
        button.GetComponent<SpellPanelTip>().spell = spell;
        button.GetComponent<Button>().SetBtnEvent(() =>
        {
            BattleCastManager.instance.SwitchModeIntoCastSpell(idName);
        });
        button.transform.localPosition = GetTargetPosition(degree);
        button.iconImage.sprite = iconFrom.Invoke(spell.iconResIdName);
    }
    public void InitSubBtn(ItemSlot button, string idName, Func<string, Sprite> iconFrom)
    {
       
        var spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(idName);
        button.GetComponent<SpellPanelTip>().spell = spell;
        button.GetComponent<Button>().SetBtnEvent(() =>
        {
            BattleCastManager.instance.SwitchModeIntoCastSpell(idName);
        });
        button.iconImage.sprite = iconFrom.Invoke(spell.iconResIdName);
    }
    public void TryPosTrigger(Vector3 targetDelta)
    {
        //拖拽成立
        GameObject targetObject = null;
        float minDistance = float.MaxValue;
        foreach (var item in subItems)
        {
            float nowDistance = Vector3.Distance(item.transform.localPosition, targetDelta);
            if (nowDistance < minDistance)
            {
                minDistance = nowDistance;
                targetObject = item.gameObject;
            }
        }
        if (targetObject != null)
        {
            targetObject.GetComponentInChildren<Button>().onClick.Invoke();
        }
    }

    public Vector3 GetTargetPosition(float degree)
    {
        Vector3 result = new Vector3(0, splitAreaLength, 0);
        Quaternion a = Quaternion.AngleAxis(degree, new Vector3(0, 0, 1));// new Vector3(0, 0, 1)
        return a * result;
    }
}
