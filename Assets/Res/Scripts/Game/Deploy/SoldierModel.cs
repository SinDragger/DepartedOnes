using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SoldierModel : MonoBehaviour
{
    [HideInInspector]
    public string modelName;
    public CommandUnit commander
    {
        get
        {
            if (nowStatus != null)
                return nowStatus.commander;
            else return null;
        }
    }
    public SoldierStatus nowStatus;
    public SpriteRenderer shadow;
    public SpriteRenderer guide;
    public Transform flipRoot;
    public ClickToMove actionModel;
    public ConcurrentDictionary<string, GameObject> effectDic = new ConcurrentDictionary<string, GameObject>();
    EquipSwitchXML equipSwitch;
    public bool needDebug;
    public EquipSwitchXML EquipSwitch
    {
        get
        {
            if (equipSwitch == null)
            {
                equipSwitch = actionModel.control.meshRenderer.GetComponent<EquipSwitchXML>();
                if (equipSwitch == null)
                {
                    equipSwitch = actionModel.control.meshRenderer.gameObject.AddComponent<EquipSwitchXML>();
                }
            }
            return equipSwitch;
        }
    }

    public virtual void Awake()
    {
        actionModel = GetComponent<ClickToMove>();

    }
    public virtual void Start()
    {

    }
    public void AddEffect(string IdName) {
        var g = GameObjectPoolManager.Instance.Spawn(DataBaseManager.Instance.configMap[IdName], transform);
        if (g != null) { 
        effectDic[IdName] = g;
        g.transform.localPosition=Vector3.zero;
        }
        
    }
    public void ActivateAllEffect() {
       
        foreach (var effect in effectDic.Keys)
        {
            var g = GameObjectPoolManager.Instance.Spawn(DataBaseManager.Instance.configMap[effect], transform);
            effectDic[effect] = g;
            g.transform.localPosition = Vector3.zero;
        }

    }
    public void RemoveEffect(string IdName) {
        if (effectDic.ContainsKey(IdName)) {
            GameObjectPoolManager.Instance.Recycle(effectDic[IdName], DataBaseManager.Instance.configMap[IdName]);
            effectDic.TryRemove(IdName,out GameObject a);
        }
    
    }
    public void CloseAllEffect()
    {
       foreach (var effect in effectDic.Keys)
        {
            GameObjectPoolManager.Instance.Recycle(effectDic[effect], DataBaseManager.Instance.configMap[effect]);
        }

    }
    public void RemoveAllEffect()
    {
        foreach (var effect in effectDic.Keys)
        {
            GameObjectPoolManager.Instance.Recycle(effectDic[effect], DataBaseManager.Instance.configMap[effect]);
        }
        effectDic.Clear();
    }
    public void ShowGuide()
    {
        guide.enabled = true;
    }

    public void HideGuide()
    {
        guide.enabled = false;
    }

    public float Distance(Vector3 checkPos)
    {
        float result = 0;
        //判断y轴差异是否大于常规Height
        if (checkPos.y > transform.position.y + 2)
        {
            //移除头顶距离差异
            checkPos.y -= 2;
            result = Vector3.Distance(checkPos, transform.position) - nowStatus.EntityData.originData.ocupySize / 2;
            if (result < 0)
            {
                return 0;//在胶囊内部
            }
            return result;
        }
        else if (checkPos.y > transform.position.y)//在范围判断内
        {
            Vector2 posXZ = new Vector2(checkPos.x, checkPos.z);
            Vector2 selfXZ = new Vector2(transform.position.x, transform.position.z);
            result = Vector2.Distance(posXZ, selfXZ) - nowStatus.EntityData.originData.ocupySize / 2;
            if (result < 0)
            {
                return 0;//在内部
            }
            return result;
        }
        result = Vector3.Distance(checkPos, transform.position) - nowStatus.EntityData.originData.ocupySize / 2;
        if (result < 0)
        {
            return 0;//在胶囊内部
        }
        return result;
    }

    public void Init(SoldierStatus status)
    {
        nowStatus = status;
        ApplyData(status.EntityData);
        if (actionModel == null)
        {
            actionModel = GetComponent<ClickToMove>();
        }
        actionModel.Init(status);
        if (status.commander != null && status.commander.belong != BattleManager.instance.controlBelong)
        {
            guide.color = ColorUtil.guideColorRed;
        }
        else
        {
            guide.color = ColorUtil.guideColorGreen;
        }

    }
    /// <summary>
    /// 进入浮空状态-关闭一堆寻路
    /// </summary>
    public void SetToFloat(bool isQuick = false)
    {
        actionModel.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        actionModel.control.selfSkelAni.state.SetAnimation(0, "Float", true);
        if (isQuick)
        {
            actionModel.control.selfSkelAni.transform.localPosition += new Vector3(0, 1f, 0);
        }
        else
        {
            actionModel.control.selfSkelAni.transform.DOLocalMoveY(actionModel.control.selfSkelAni.transform.localPosition.y + 1f, 0.1f);

        }
    }

    public void ResetFloat(bool isQuick = false)
    {
        actionModel.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        //actionModel.control.selfSkelAni.state.SetAnimation(0, "Float", true);
        if (isQuick)
        {
            actionModel.control.selfSkelAni.transform.localPosition -= new Vector3(0, 1f, 0);
        }
        else
        {
            actionModel.control.selfSkelAni.transform.DOLocalMoveY(actionModel.control.selfSkelAni.transform.localPosition.y - 1f, 0.1f);

        }
    }

    public bool IsMoving => actionModel.agentMoving;


    public virtual void Update()
    {
        if (nowStatus == null) return;
        if (nowStatus.nowHp <= 0) return;
        //Debug.Log(commander.NowMorale);
        //使用轮询来进行敌我判断||筛选敌人————只有发生位移与体型变换时才发生
        if (IsOcupyChange())
            UpdateGridOccupy();
        //筛选可攻击范围+离自己最近的作为敌人
        if (nowStatus.focusTarget != null)
        {
            if (nowStatus.focusTarget.nowHp <= 0 || nowStatus.focusTarget.nowState < 0)
            {
                nowStatus.focusTarget = null;
                return;
            }
            if (actionModel == null) actionModel = GetComponent<ClickToMove>();
            if (!actionModel.IsAttacking)//上次攻击状态完毕
            {
                //判断在攻击距离里
                if (nowStatus.attackCoolDown <= 0f && nowStatus.IsTargetInAttackRange())
                {
                    actionModel.AttackTarget(nowStatus.focusTarget.model.transform);
                }
                else
                {
                    actionModel.SetPositionAttackRangeCloseTo(nowStatus.focusTarget.model);//靠近目标并再下一帧继续尝试攻击
                }
            }
            //尝试靠近
        }
    }
    public Vector3 lastPosition;
    protected bool IsOcupyChange()
    {
        bool result = false;
        if (lastPosition != transform.position)
        {
            lastPosition = transform.position;
            nowStatus.SetObjectValue(Constant_AttributeString.POS, lastPosition);
            result = true;
        }
        //增加体积变化判断
        return result;
    }

    public virtual float DistanceTo(Vector3 worldPos)
    {
        float basicDistance = Vector3.Distance(transform.position, worldPos);
        basicDistance -= nowStatus.EntityData.originData.ocupySize / 2;
        if (basicDistance < 0f) basicDistance = 0f;
        return basicDistance;
    }

    public virtual void ChangeFaceTo(Vector3 vector)
    {
        flipRoot.transform.forward = vector;
    }
    public void FaceToLeft()
    {
        actionModel.ShiftModelFaceTo(false);
        //  flipRoot.GetComponent<ForceFaceCamera>().FaceToLeft();
    }
    public void FaceToRight()
    {
        actionModel.ShiftModelFaceTo(true);
    }
    protected virtual void ApplyData(TroopEntity originData)
    {
        //actionModel.control.selfSkelAni.timeScale = 1.5f;
        var agent = actionModel.m_Agent;
        //TODO：之后尝试剥离Agent只留路径寻路逻辑进行
        agent.speed = originData.speed;
        agent.angularSpeed = 360;
        agent.acceleration = originData.speed * 10;
        agent.stoppingDistance = originData.speed / 100;
    }

    public void SetSpeed(float value)
    {
        var agent = actionModel.m_Agent;
        agent.speed = value;
        agent.acceleration = value * 10;
        agent.stoppingDistance = value / 200;
    }

    public void SetSpeedPercent(float value)
    {
        actionModel.control.SetMoveSpeedAnimPercent(Mathf.Lerp(value, 1f, 0.3f));
    }

    public virtual void OnReset()
    {
        nowStatus = null;
        HideGuide();
        actionModel.OnReset();
        actionModel.enabled = true;
        EquipSwitch.ClearEquipSet();
        occupyMapGrids.Clear();
        //TODO 加个移除特效表现的方法
        //GameObjectPoolManager.Instance.Recycle(s.model.gameObject, "Prefab/" + s.model.modelName);

    }

    public virtual void GetHitted(int attackPower)
    {
        //nowStatus.GetHurt(attackPower);
        //判断是否死亡/转切阶段触发技能
    }

    public void Die()
    {
        CloseAllEffect();
        actionModel.OnDying();
        HideGuide();
        RemoveGridOccupy();
        //关闭
        actionModel.enabled = false;
    }


    public HashSet<BaseGrid> occupyMapGrids = new HashSet<BaseGrid>();
    HashSet<BaseGrid> temp = new HashSet<BaseGrid>();
    [BurstCompile]
    public virtual void UpdateGridOccupy()
    {
        if (nowStatus.commander == null) return;
        HashSet<BaseGrid> mapGrids = GridMapManager.instance.gridMap.GetGridsInCircle(transform.position, nowStatus.EntityData.originData.ocupySize);
        temp.Clear();
        ////将自己点和坐标传入以获取对应
        foreach (var grid in occupyMapGrids)
        {
            if (!mapGrids.Remove(grid))//新有 旧无
            {
                grid.GetGridContain<SoldierModel>().Remove(this);
                temp.Add(grid);
            }
        }
        foreach (var grid in temp)
        {
            occupyMapGrids.Remove(grid);
        }
        //残留的新的
        foreach (var grid in mapGrids)
        {
            occupyMapGrids.Add(grid);
            nowStatus.commander.OcupyGrid(grid);
            grid.GetGridContain<SoldierModel>().Add(this);
        }
    }
    GridMapCorpse selfCorpse;
    public virtual void RemoveGridOccupy()
    {
        var corpse = new GridMapCorpse();
        selfCorpse = corpse;
        corpse.Init(nowStatus);
        if (occupyMapGrids.Count == 0)
        {
            IsOcupyChange();
            UpdateGridOccupy();
        }
        //把自己变成一具尸体进行Item的存储
        foreach (var grid in occupyMapGrids)
        {
            grid.GetGridContain<SoldierModel>().Remove(this);
            grid.GetGridContain<GridMapCorpse>().Add(corpse);
        }
        //occupyMapGrids.Clear();
    }
    /// <summary>
    /// 脱离死亡状态
    /// </summary>
    public void ReverseDeath()
    {
        selfCorpse.RemoveGridOccupy();
        selfCorpse = null;
    }
}
