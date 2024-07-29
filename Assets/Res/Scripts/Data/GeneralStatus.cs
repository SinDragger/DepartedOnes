using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralStatus : IAggregationDataInject
{

    public List<SkillData> skillDatas;

    public SkillData itemSkillData;

    public bool InjectData(AggregationEntity entity)
    {
        throw new System.NotImplementedException();
    }
}
