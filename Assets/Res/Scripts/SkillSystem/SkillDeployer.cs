using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能释放器
/// </summary>
public abstract class SkillDeployer : MonoBehaviour
{
    [SerializeField]
    private SkillData skillData;

    private IAttackSelector selector;

    private IImpactEffect[] impacts => skillData.effects;

    public event Action updateEvent;

    public event Action exitEvent;

    public event Action fixedUpdateEvent;

    public SkillData SkillData
    {
        get
        {
            return skillData;
        }

        set
        {
            skillData = value;
            //初始化释放器
            InitDeployer();
        }
    }
    private void OnDisable()
    {
        exitEvent?.Invoke();
    }

    private void Update()
    {
        updateEvent?.Invoke();


    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    //初始化释放器
    private void InitDeployer()
    {
        //创建选区算法对象

        selector = DeployerConfigFactory.CreateAttackSelector(SkillData);

        //创建技能效果算法对象
        //impacts = DeployerConfigFactory.CreateImpactEffects(SkillData);

    }

    /// <summary>
    /// 以技能对象为中心的查询敌人
    /// </summary>
    public void CalculateTargets()
    {

        skillData.attackTargets = selector.SelectTarget(skillData, transform);

    }
    /// <summary>
    /// 以施法者为中心的查询敌人
    /// </summary>
    /// <param name="transform">施法者的transfrom</param>
    public void CalculateTargets(Transform transform)
    {

        skillData.attackTargets = selector.SelectTarget(skillData, transform);

    }

    /// <summary>
    /// 执行技能效果
    /// </summary>
    public void ImapactTargets()
    {
        for (int i = 0; i < impacts.Length; i++)
        {
            impacts[i].Execute(this);
        }

    }


    //执行算法对象 
    public abstract void DeploySkill();
    //释放方式


    public void DestorySkillDeployer()
    {
        Destroy(gameObject);
    }
}
