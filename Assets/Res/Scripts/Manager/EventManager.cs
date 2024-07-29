using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    Dictionary<string, List<Action<EventData>>> listenDic = new Dictionary<string, List<Action<EventData>>>();

    public void RegistEvent(object id, Action<EventData> callback)
    {
        RegistEvent(id.ToString(), callback);
    }

    public void RegistEvent(string id, Action<EventData> callback)
    {
        if (!listenDic.ContainsKey(id))
        {
            listenDic[id] = new List<Action<EventData>>();
        }
        if (!listenDic[id].Contains(callback))
            listenDic[id].Add(callback);
    }

    public void UnRegistEvent(object id, Action<EventData> callback)
    {
        UnRegistEvent(id.ToString(), callback);
    }

    public void UnRegistEvent(string id, Action<EventData> callback)
    {
        if (!listenDic.ContainsKey(id))
        {
            return;
        }
        listenDic[id].Remove(callback);
    }

    public void DispatchEvent(EventData eventData)
    {
        if (listenDic.ContainsKey(eventData.eventId))
        {
            foreach (var callback in listenDic[eventData.eventId])
            {
                callback.Invoke(eventData);
            }
        }
        ObjectPoolManager.Instance.Recycle<EventData>(eventData);
    }

    public void ProcessMapEvent(string[] param, Action callback)
    {
        var legion = LegionManager.Instance.nowLegion;
        UnitData unitData = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(param[2]);
        if (param[1] == "RaiseGetTroop")
        {
            UIManager.Instance.ShowUI("RaiseGetTroopPanel", (r) =>
            {
                (r as RaiseGetTroopPanel).Init(legion, unitData, callback);
            });
        }
    }

    public void ProcessMapEvent(EventTriggerData eventData, Action callback = null)
    {
        //var legion = LegionManager.Instance.nowLegion;
        eventData.Process();
        //UnitData unitData = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(param[2]);
        //if (param[1] == "RaiseGetTroop")
        //{
        //    UIManager.Instance.ShowUI("RaiseGetTroopPanel", (r) =>
        //    {
        //        (r as RaiseGetTroopPanel).Init(legion, unitData,callback);
        //    });
        //}
    }
}
/// <summary>
/// 中转处理事件模块
/// </summary>
public class EventProcessModel
{
    public EventProcessModel transitTarget;
    Dictionary<string, List<Action<EventData>>> listenDic = new Dictionary<string, List<Action<EventData>>>();

    public void RegistEvent(object id, Action<EventData> callback)
    {
        RegistEvent(id.ToString(), callback);
    }

    public void RegistEvent(string id, Action<EventData> callback)
    {
        if (!listenDic.ContainsKey(id))
        {
            listenDic[id] = new List<Action<EventData>>();
        }
        listenDic[id].Add(callback);
    }

    void DispatchEvent(EventData eventData)
    {
        if (listenDic.ContainsKey(eventData.eventId))
        {
            foreach (var callback in listenDic[eventData.eventId])
            {
                callback.Invoke(eventData);
            }
        }
        if (transitTarget != null)
        {
            transitTarget.DispatchEvent(eventData);
        }
        else
        {
            EventManager.Instance.DispatchEvent(eventData);
        }
    }

    public void FireEvent(object eventId, params object[] args) { FireEvent(eventId.ToString(), args); }
    public void FireEvent(string eventId, params object[] args)
    {
        EventData result = ObjectPoolManager.Instance.Spawn<EventData>();
        result.eventId = eventId;
        for (int i = 0; i < args.Length; i += 2)
        {
            result.SetValue(args[i].ToString(), args[i + 1]);
        }
        DispatchEvent(result);
    }
}

/// <summary>
/// 可回收
/// </summary>
public class EventData
{
    public string eventId;
    Dictionary<string, object> valueDic = new Dictionary<string, object>();
    public EventData() { }
    public EventData(object eventType) { eventId = eventType.ToString(); }
    public EventData(object eventType, params object[] values)
    {
        eventId = eventType.ToString();
        for (int i = 0; i < values.Length; i += 2)
        {
            SetValue(values[i], values[i + 1]);
        }

    }

    public T GetValue<T>(object key) { return GetValue<T>(key.ToString()); }
    public T GetValue<T>(string key)
    {
        if (valueDic.ContainsKey(key))
        {
            return (T)valueDic[key];
        }
        return default;
    }
    public void SetValue<T>(object key, T value) { SetValue<T>(key.ToString(), value); }
    public void SetValue<T>(string key, T value)
    {
        valueDic[key] = value;
    }
}

public enum MapEventType
{
    LEGION_SELECT,
    CONSTRUCTION_SELECT,
    SECTOR_SELECT,
    TROOP_RECRUIT,//部队征召

    CONSTRUCTION_INTERACTIVE,//建筑交互
}

public enum GameEventType
{
    DAMAGE,//伤害造成
    HEAL,//伤害治愈
    BUFF_ADD,//效果附加
    SOLDIER_DIE,//士兵死亡
    TROOP_ELIMINATE,//部队消亡

    OnPlayerBattleWin,//战斗结束
    OnPlayerGetTroop, //玩家获得军队
    OnPlayerConfigEquip,//玩家调整装备
    OnConstructionBeStatetioned,//建筑被军队驻扎
}