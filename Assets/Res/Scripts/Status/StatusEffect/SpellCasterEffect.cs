using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 咒语施法效果
/// </summary>
public class SpellCasterEffect : StatusEffectTerm
{
    /// <summary>
    /// 目标类型
    /// </summary>
    public int targetType;
    /// <summary>
    /// 施法的法术Id
    /// </summary>
    public string spellId;
    /// <summary>
    /// 值
    /// </summary>
    public int value;
    public float countDownTime;
    CommandUnit command;
    public override void Execution(AggregationEntity target)
    {
        command = target as CommandUnit;
        GameManager.timeRelyMethods += TimeCountDown;
        UIManager.Instance.ShowUI("CastRemind", (ui) =>
        {
            (ui as CastRemind).Init(command, spellId, countDownTime);
        });
    }

    void TimeCountDown(float deltaTime)
    {
        if (command.TroopState == TroopState.ENGAGING || command.TroopState == TroopState.MOVING) return;
        var prefIdName = $"SpellCast_{spellId}_CountDownTime";
        float readyTime = command.GetFloatValue(prefIdName);
        readyTime += deltaTime;
        if (readyTime > countDownTime)
        {
            readyTime -= countDownTime;
            SpellCast();
        }
        command.SetFloatValue(prefIdName, readyTime);
    }

    void SpellCast()
    {
        BattleCastManager.instance.TroopCastSpell(command, targetType, spellId);
    }

    public override void ReverseExecution(AggregationEntity target)
    {
        GameManager.timeRelyMethods -= TimeCountDown;
    }
}
