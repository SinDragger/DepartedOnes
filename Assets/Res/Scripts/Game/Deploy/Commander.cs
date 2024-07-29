using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander
{
    public int belong;
    public List<CommandUnit> ableCommands = new List<CommandUnit>();
    public Commander nowEnermy;
    //判断敌人的状态
    public int judgeEnermyState;
    //确认部队的中心
    //往敌人中心点进行行军
    //修正各自部队的期望移速(同步)
    //给出朝向与排序的偏转
    public Vector3 centorPos;
    /// <summary>
    /// 自动冲锋的序列
    /// </summary>
    public HashSet<TroopEntity> autoChargeSet = new HashSet<TroopEntity>();

    public void UpdateCommand(float deltaTime)
    {
        if (nowEnermy == null)
            UnitControlManager.instance.GetCommandEnermy(this);
        for (int i = 0; i < ableCommands.Count; i++)
        {
            ableCommands[i].UpdateCommand(deltaTime);
            //if (BattleManager.autoBattle && ableCommands[i].belong != BattleManager.instance.controlBelong)
            //{
            //    ableCommands[i].TriggerCommandToAggressive();
            //}
            if (autoChargeSet.Contains(ableCommands[i].entityData))
            {
                if (ableCommands[i].TroopState == TroopState.TAKE_OVER || ableCommands[i].TroopState == TroopState.WAITING)
                    ableCommands[i].SetTarget(UnitControlManager.instance.GetEnermyInRange(ableCommands[i], 500f));
            }
        }
    }

    public void AddAutoCharge(TroopEntity entity)
    {
        autoChargeSet.Add(entity);
    }

    public void SetCenterPos(Vector3 centorPos)
    {
        int count = 0;
        for (int i = 0; i < ableCommands.Count; i++)
        {
            count++;
            centorPos += ableCommands[i].lastPosition / count;
        }
    }
}
