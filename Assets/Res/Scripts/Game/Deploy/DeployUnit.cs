using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 进行部队的部署
/// </summary>
public class DeployUnit : MonoBehaviour
{
    /// <summary>
    /// 所属势力
    /// </summary>
    public int belong;
    /// <summary>
    /// 单位ID
    /// </summary>
    public string unitId;
    /// <summary>
    /// 物种类型
    /// </summary>
    public string speciesType;
    /// <summary>
    /// 物种类型
    /// </summary>
    public string subSpeciesType;

    public string targetCommandId;
    //其他的配置内容
    /// <summary>
    /// 可配置的数量
    /// </summary>
    public int unitNum;
    /// <summary>
    /// 是否等待
    /// </summary>
    public int waitCall;

    public bool autoAttack;
    public bool autoDie;
    public bool singleAggressiveTrigger;
    // Start is called before thae first frame update
    void Start()
    {
        bool isDifficult = DataBaseManager.Instance.GetTempData<bool>("UseDiffcultMode");
        CoroutineManager.StartWaitUntil(() => BattleManager.instance && BattleManager.instance.hasInit, () =>
         {
             if (waitCall == 0)
             {
                 if (string.IsNullOrEmpty(targetCommandId))
                 {
                     var command = UnitControlManager.instance.DeployTroop(transform.position, transform.forward, unitId, speciesType, subSpeciesType, belong, unitNum);
                     if (autoAttack || isDifficult)
                         command.TriggerCommandToAggressive();
                     if (autoDie)
                     {
                         command.AutoDie();
                     }
                     if (singleAggressiveTrigger)
                     {
                         command.aggressiveTrigger = null;
                     }
                 }
                 else
                 {
                     var command = UnitControlManager.instance.DeployUnitToTargetCommand(transform.position, transform.forward, unitId, speciesType, subSpeciesType, belong, targetCommandId);
                     if (autoDie)
                     {
                         command.AutoDie();
                     }
                     if (singleAggressiveTrigger)
                     {
                         command.aggressiveTrigger = null;
                     }
                 }
                     Destroy(gameObject);
             }
         });
        //TODO变成异步逻辑
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "DeployFlag", true, GetGizmosColor());
    }

    Color GetGizmosColor()
    {
        switch (belong)
        {
            case 0: return Color.gray;
            case 1: return Color.blue;
            case 2: return Color.red;
            case 3: return Color.yellow;
        }
        return Color.white;
    }
#endif
}
