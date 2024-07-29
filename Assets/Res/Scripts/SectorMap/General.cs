using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class General : AggregationEntity
{
    public int belong;
    public int maxLife;
    public int life;
    public GeneralData sourceData;
    //技能列表
    public List<SkillData> skillDatas = new List<SkillData>();
    //TODO 卷轴技能
    //public SkillData itemSkillData=new SkillData();
    public List<ItemData> itemDatas = new List<ItemData>();

    public List<EntityStatus> status = new List<EntityStatus>();
    /// <summary>
    /// 释放技能所需能量
    /// </summary>

    public float nowEnergy
    {
        get
        {

            return GetFloatValue(Constant_AttributeString.GENERAL_ENERGY);
        }
        set
        {

            SetFloatValue(Constant_AttributeString.GENERAL_ENERGY, value);
        }
    }

    public float nowMaxEnergy
    {
        get
        {

            return GetFloatValue(Constant_AttributeString.GENERAL_ENERGYMAX);
        }
        set
        {

            SetFloatValue(Constant_AttributeString.GENERAL_ENERGYMAX, value);
        }
    }


    public General(GeneralData data)
    {
        sourceData = data;
        var unit = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(data.unitType);
        life = unit.maxLife;
        maxLife = unit.maxLife;
        nowEnergy = data.maxEnergy;
        nowMaxEnergy = data.maxEnergy;
        if (sourceData.selfStatusAttachs != null)
        {
            for (int i = 0; i < sourceData.selfStatusAttachs.Length; i++)
            {
                status.Add(StatusManager.Instance.RequestStatus(sourceData.selfStatusAttachs[i], this, belong));
            }
        }
        if (data.skillIdNames != null)
        {
            for (int i = 0; i < data.skillIdNames.Length; i++)
            {
                SkillData skillData = DataBaseManager.Instance.GetIdNameDataFromList<SkillData>(data.skillIdNames[i]);
                skillDatas.Add(skillData);
            }
        }
        //给将军每秒5点回复
        SetFloatValue(Constant_AttributeString.GENERAL_ENERGY_RECOVERY_VALUE, 5f);
    }

    float smallPercent;
    /// <summary>
    /// 生命恢复
    /// </summary>
    public bool LifeRecovery(float deltaTime)
    {
        if (life < maxLife)
        {
            float recoveryTime = 6;
            smallPercent += deltaTime / 3600f * recoveryTime;
            if (smallPercent > 1f)
            {
                life += (int)(smallPercent / 1f);
                smallPercent %= 1f;
                if (life > maxLife)
                {
                    life = maxLife;
                }
                return true;
            }
        }
        return false;
    }

    public float LifeLostPercent => (float)(maxLife - life) / (float)maxLife * 100;

    public void AttachStatusToSoldier(SoldierStatus soldier)
    {

        if (sourceData.statusAttachs != null)
        {
            for (int i = 0; i < sourceData.statusAttachs.Length; i++)
            {

                soldier.status.Add(StatusManager.Instance.RequestStatus(sourceData.statusAttachs[i], soldier, belong));

            }
        }
    }

    public Sprite GetHeadIcon()
    {
        return DataBaseManager.Instance.GetSpriteByIdName(sourceData.headIconTexIdName);
    }
}
