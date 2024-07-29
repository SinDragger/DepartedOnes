using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// 
/// </summary>
public class DeployerConfigFactory
{

    public static IAttackSelector CreateAttackSelector(SkillData skillData)
    {
        //选取类
        //选取对象命名规则 SkillData.selectorType枚举名加啊AttackSelector
        //例如扇形选区命名为  SectorAttackSelector
        string className = string.Format("{0}AttackSelector", skillData.selectorType.ToString()); ;
        return CreateObject<IAttackSelector>(className);
    }

    private static T CreateObject<T>(string className) where T : class
    {

        Type type = Type.GetType(className);
        return Activator.CreateInstance(type) as T;

    }



}
