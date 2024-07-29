using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能释放之前的准备 放在Model身上在创建Model实例时通过Status获取到List<SkillData> skills
/// </summary>
public class CharacterSkillManager : MonoBehaviour
{
    public List<SkillData> skills;

    //itemSkill的技能冷却也解决了
    //public List<SkillData> itemSkills;

    public List<ItemData> itemDatas;

    public SpineUnityControl control;

    public bool canCastSkill = true;
    /// <summary>
    /// 整个CharacterSkillManager的所以数据来源
    /// </summary>
    public General general;
    public float CharacterEnergy { get { return general.nowEnergy; } set { general.nowEnergy = value; } }

    private void Awake()
    {
        control = GetComponent<SpineUnityControl>();
    }

    public void SetGeneral(General general)
    {
        this.general = general;
        InitSkill(general.skillDatas);
        InitItemSkill(general.itemDatas);

    }

    /// <summary>
    /// 放在生成Model
    /// </summary>
    /// <param name="skills"></param>
    public void InitSkill(List<SkillData> skills)
    {
        this.skills = skills;
        if (skills == null)
        {
            return;
        }
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].skillPrefab = Resources.Load<GameObject>(DataBaseManager.Instance.configMap[skills[i].prefabName]);
            skills[i].owner = gameObject;
        }

    }

    public void InitItemSkill(List<ItemData> itemDatas)
    {
        this.itemDatas = itemDatas;
        if (itemDatas == null)
        {
            return;
        }
        for (int i = 0; i < itemDatas.Count; i++)
        {
            this.itemDatas[i].skillData.skillPrefab = Resources.Load<GameObject>(DataBaseManager.Instance.configMap[this.itemDatas[i].skillData.prefabName]);
            this.itemDatas[i].skillData.owner = gameObject;
        }

    }

    /// <summary>
    /// 物品技能释放条件
    /// </summary>
    /// <returns></returns>
    public SkillData PrepareItemSkill(int id)
    {
        //
        ItemData itemData = itemDatas.Find(s => s.itemID == id);
        if (itemData.skillUsageFrequency > 0 && itemData.skillData.coolRemain <= 0)
        {
            itemData.skillUsageFrequency--;
            return itemData.skillData;
        }
        else return null;
    }

    //技能释放条件 不处于冷却状态
    [Obsolete("释放技能消耗资源还没做判断")]
    public SkillData PrepareSkill(int id)
    {
        //根据ID查找技能数据
        SkillData data = skills.Find(s => s.skillID == id);
        foreach (var item in data.effects)
        {
            if (!item.Releasable()) {
                return null;
            }
        }
       
        // TODO 判断条件 CD 法力值 是否可以施法
        if (data != null && data.coolRemain <= 0 && CharacterEnergy >= data.costSP && canCastSkill)
        {
            CharacterEnergy -= data.costSP;
            return data;
        }
        else
            return null;
        //返回技能数值
    }


    //生成技能
    public void GenerateSkill(SkillData data)
    {

        //生成预制件 对象池生成
        GameObject skillGo = Instantiate(data.skillPrefab, transform.position, transform.rotation);
        skillGo.transform.parent = transform;
        SkillDeployer deployer = skillGo.GetComponent<SkillDeployer>();
        deployer.SkillData = data;
        deployer.DeploySkill();
        //销毁预制件TODO 对象池管理 创建对象的回收通过对象池完成
        //Destroy(skillGo, data.durationTime + 0.1f);
        //技能 进入cd
    }


    private void Update()
    {
        if (skills != null)
        {
            foreach (var skill in skills)
            {
                if (skill.coolRemain > 0)
                {
                    skill.coolRemain -= Time.deltaTime;
                    if (skill.coolRemain < 0) skill.coolRemain = 0;
                }
            }
        }
        if (itemDatas != null)
        {
            //if (itemSkills.coolRemain > 0)
            //{
            //    itemSkill.coolRemain -= Time.deltaTime;
            //}
            //else itemSkill.coolRemain = 0;
            foreach (var item in itemDatas)
            {
                if (item.skillData.coolRemain > 0)
                {
                    item.skillData.coolRemain -= Time.deltaTime;
                    if (item.skillData.coolRemain < 0) item.skillData.coolRemain = 0;
                }
            }

        }
    }

    /// <summary>
    /// 还要处理动画播放
    /// </summary>
    /// <returns></returns>
    public void OnSkillDown1()
    {
        //TODO 获取当前按键对应技能信息
        SkillData data = PrepareSkill(skills[0].skillID);

        //如果技能获得的技能信息不为空
        if (data != null)
        {
            //处理 动画
            control.PlayAnim(data.animationName);
            //释放 技能
            GenerateSkill(data);
        }

    }
    public void OnSkillDown2()
    {
        if (skills.Count < 2) return;
        SkillData data = PrepareSkill(skills[1].skillID);
        if (data != null)
        {
            control.PlayAnim(data.animationName);
            GenerateSkill(data);


        }

    }
    public void OnSkillDown3()
    {
        if (skills.Count < 3) return;
        SkillData data = PrepareSkill(skills[2].skillID);
        if (data != null)
        {
            control.PlayAnim(data.animationName);
            GenerateSkill(data);

        }

    }
    /// <summary>
    /// 测试 设置为冲锋 模拟卷轴技能效果
    /// </summary>
    public void OnSkillDown4()
    {
        if (skills.Count < 4) return;
        SkillData data = PrepareSkill(skills[3].skillID);

        if (data != null)
        {
            control.PlayAnim(data.animationName);
            GenerateSkill(data);


        }
        //SkillData data = PrepareItemSkill(itemDatas[0].itemID);
        //if (data != null && (canCastSkill))
        //{
        //    control.PlayAnim(data.animationName);
        //    GenerateSkill(data);
        //}
    }

}
