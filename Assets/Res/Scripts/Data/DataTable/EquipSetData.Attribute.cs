using System;
using System.Collections.Generic;

/// <summary>
/// 套装装备数据
/// </summary>
public partial class EquipSetData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 生产所需
    /// </summary>
    public EntityStack[] Cost;
    public string Material;
    public string EquipType;
    public int isUnReachable;
    public int Weight;
    public int WorkLoad;
    public string[] Crafts;

    //伤害 Damage
    public int Damage => GetIntValue("Damage");
    //破甲 Break
    public int Break => GetIntValue("Break");
    //暴击 Critical
    public int Critical => GetIntValue("Critical");

    //武器速度 WeaponSpeed
    public float WeaponSpeed => GetFloatValue("WeaponSpeed", 1f);
    //攻击范围 AttackRange
    public float AttackRange => GetFloatValue("AttackRange", 0.1f);
    //射击预设 AmmoIDName
    public string AmmoIDName => GetStringValue("AmmoIDName", "");

    //坚韧 Hardness
    public int Hardness => GetIntValue("Hardness");
    //抗性 Resistance
    public int Resistance => GetIntValue("Resistance");
    //覆盖 Coverage
    public int Coverage => GetIntValue("Coverage");
    //行动阻碍 Obstruction
    public int Obstruction => GetIntValue("Obstruction");
    //材料效能 MaterialAffect
    public float MaterialAffect => GetFloatValue("MaterialAffect", 1f);
}