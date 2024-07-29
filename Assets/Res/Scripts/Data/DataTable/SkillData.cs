using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillData: AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 技能ID
    /// </summary>
    public int skillID;
    /// <summary>
    /// 技能名字
    /// </summary>
    public string skillName;
    /// <summary>
    /// 技能描述
    /// </summary>
    public string skillDescription;
    /// <summary>
    /// 技能冷却时间
    /// </summary>
    public float coolTime;
    /// <summary>
    /// 技能当前剩余时间
    /// </summary>
    public float coolRemain;
    /// <summary>
    /// 技能消耗
    /// </summary>
    public float costSP;
    /// <summary>
    /// 技能作用范围
    /// </summary>
    public float atackDistance;
    /// <summary>
    /// 技能角度
    /// </summary>
    public float attackAngle;
    /// <summary>
    /// 检测获得作用范围内的作用对象数组
    /// </summary>
    [HideInInspector]
    public SoldierModel[] attackTargets;
    /// <summary>
    /// 连击的下一个技能
    /// </summary>
    public int nextBatterld;
    /// <summary>
    /// 伤害比例 他的伤害计算管线是 skillData.atkRatio*skillData.owner.GetComponent<SoldierModel>().当前角色数据.攻击力;
    /// </summary>
    public float atkRatio;
    /// <summary>
    /// 技能持续时间 //动态技能持续时间时间没法做到
    /// </summary>
    public float durationTime;
    /// <summary>
    /// 技能攻击间隔
    /// </summary>
    public float atkInterval;
    /// <summary>
    /// 技能所属
    /// </summary>
    [HideInInspector]
    public GameObject owner;
    /// <summary>
    /// 技能预制件名称
    /// </summary>
    public string prefabName;
    /// <summary>
    /// 技能预制件
    /// </summary>
   // [HideInInspector]
    public GameObject skillPrefab;
    /// <summary>
    /// 播放动画名称
    /// </summary>
    public string animationName;
    /// <summary>
    /// 受击特效名称
    /// </summary>
    public string relatedEffectName;
    /// <summary>
    /// 技能等级
    /// </summary>
    public int level;
    /// <summary>
    /// 攻击类型
    /// </summary>
    public SkillAttackType attackType;
    /// <summary>
    /// 攻击范围类型
    /// </summary>
    public SelectorType selectorType;

    public IImpactEffect[] effects;

    /// <summary>
    /// 写xml文档
    /// </summary>
    /// <returns></returns>
    public string PrintXML()
    {
      
        // 写一个通过反射 完成XML文件生成的通用方法 通过传入type 完成
        
        return "";
    }

}

[Serializable]
public enum SkillAttackType
{
    /// <summary>
    /// 单体
    /// </summary>
    Single,
    /// <summary>
    /// 群体
    /// </summary>
    Group

}
[Serializable]
public enum SelectorType
{
    /// <summary>
    /// 扇形
    /// </summary>
    Sector,
    /// <summary>
    /// 矩形
    /// </summary>
    Rectangle,


}
