using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class ARPGControl : MonoBehaviour
{
    public int ID;
    public SoldierModel m_SoldierModel;
    public SC_Common control;
    public CharacterSkillManager characterSkillManager;
    public NavMeshAgent m_Agent;
    public ITakeOverAbleMove moveModule;
    //public bool canAutoAttack = true;

    public bool canAttack => !control.isAttacking;
    //{
    //    get
    //    {

    //        return canAutoAttack;
    //    }
    //    set
    //    {

    //        canAutoAttack = value;
    //    }
    //}

    //需要结构性调整
    public General general;

    [SerializeField]
    bool canSetDestantion = true;

    public ParticleSystem p;
    public static void AttachARPGControl(GameObject target, General general)
    {
        //对其附加ARPG相关的脚本 AddComponent  顺序不能不能遍 ARPGControl的Awake中有getcomponent<CharacterSkillManager>();
        target.AddComponent<CharacterSkillManager>();
        ARPGControl arpgControl = target.AddComponent<ARPGControl>();
        //添加将军角色的Model 目前主要的用途是 做技能 和卷轴的数据
        //TODO 之后读表进行赋值 
        arpgControl.ID = 123;
        if (general.skillDatas != null)
        {
            target.GetComponent<CharacterSkillManager>()?.SetGeneral(general);

        }
        else
            Debug.LogError("技能初始化异常查看GeneralModel");
        //target.AddComponent<GeneralModel>().general = general;
        ARPGManager.Instance.currentGeneralControl = arpgControl;
        arpgControl.general = general;
        general.AttachStatusToSoldier(target.GetComponent<SoldierModel>().nowStatus);
        //赋予soldierStatus General的技能

    }



    public static void RemoveARPGControl(GameObject target)
    {
        Destroy(target.GetComponent<CharacterSkillManager>());
        Destroy(target.GetComponent<ARPGControl>());
    }

    public float Auto_Attack_Range = 0.6f;

    public float Attack_Area = 0.5f;

    void Update()
    {
        general.life = m_SoldierModel.nowStatus.nowHp;
        if (!ARPGManager.Instance.isActive) return;
        Move();
        //AttackSurroundTarget();
    }
    public float energyRecoverAmount;

    public ARPGControl()
    {
        ARPGManager.Instance.generals[ID] = this;
    }
    void Awake()
    {
        m_SoldierModel = GetComponent<SoldierModel>();
        control = GetComponent<SC_Common>();//
        characterSkillManager = GetComponent<CharacterSkillManager>();
        m_Agent = GetComponent<NavMeshAgent>();
        moveModule = GetComponent<ITakeOverAbleMove>();

        m_Agent.speed = 5;
        //if (GameManager.instance.defaultARPG)
        //{
        //    ARPGManager.Instance.SetCurrentGeneralControl();
        //    ARPGManager.Instance.Active();
        //    GameManager.instance.defaultARPG = false;
        //}
    }
    private void OnEnable()
    {
        m_SoldierModel = GetComponent<SoldierModel>();
        m_SoldierModel.nowStatus.onHit -= OnControlTargetHit;

        m_SoldierModel.nowStatus.onHit += OnControlTargetHit;

    }
    void Start()
    {
        var CharacterUI = UIManager.Instance.GetUI("ARPGCharacterUI") as ARPGCharacterUI;
        CharacterUI.Init(m_SoldierModel.nowStatus, general);
        CharacterUI.OnShow();
        Debug.LogError("Start");
        string[] spellArray = m_SoldierModel.commander.GetCastableSpells();
        if (spellArray != null && spellArray.Length > 0 && m_SoldierModel.commander.aliveCount > 0 && ARPGManager.Instance.canShowSkillUI)
        {
            //BattleCastManager.instance.SetCastCommandUnit(m_SoldierModel.commander, spellArray);
            //打开UI 传入Caommand
            ShowUI();
          
        }
        m_SoldierModel.nowStatus.onDie += OnControlDie;
    }

    public void ShowUI()
    {
        var battleGeneralSpellListUI = UIManager.Instance.GetUI("BattleGeneralSpellListUI") as BattleGeneralSpellListUI;
        battleGeneralSpellListUI.Init(m_SoldierModel.commander);
        battleGeneralSpellListUI.OnShow();
    }
    public void Active()
    {
        //TODO 找到所有的inputModule在激活之后将其添加到对应的聚合当中
        CameraControl.Instance.ChangeCameraToARPG(this);
        //通过ARPGManager中的currentGeneralControl获取当前的transfrom
        moveModule.TakeOver();
        UnitControlManager.instance.HideControlUI(m_SoldierModel.nowStatus.commander);
        control.PlayIdle();
        //将当前将军的navmesh的priority改变
        m_SoldierModel.actionModel.m_Agent.avoidancePriority = 10;
        m_SoldierModel.commander.RemoveAttackTarget();
        m_SoldierModel.commander.canSetTarget = false;
    }
    public void OnControlDie(SoldierStatus soldier)
    {
        //是否需要添加不同的死亡情况 还是说将不同的死亡情况的监听开放出去给监听脚本去执行？？
        ResetMoveVector();
        CanSetDestantion();
        moveModule.ReturnBack();

        var battleGeneralSpellListUI = UIManager.Instance.GetUI("BattleGeneralSpellListUI") as BattleGeneralSpellListUI;
        battleGeneralSpellListUI.OnHide();

        //死亡之后退出ARPG 模式
        ARPGManager.Instance.currentGeneralControl = null;
        ARPGManager.Instance.generals.Remove(ID);
        m_SoldierModel.commander.canSetTarget = true;

        m_SoldierModel.actionModel.m_Agent.avoidancePriority = 50;
    }



    Vector3 targetTo;
    /// <summary>
    /// TODO:优化进战斗底层
    /// </summary>
    public bool AttackSurroundTarget()
    {
        if (canAttack)
        {
            moveSpeed = 0.5f;
            control.AttachEventListener("Hit", AutoAttack);
            //arpgControl.m_SoldierModel.actionModel.FixFaceTarget(获取到最近的敌人.transform.position - arpgControl.transform.position);
            //如果没有在移动 搜索最近的敌人并转向攻击
            if (!startMove)
            {
                //搜索
                var newSetModel = GridMapManager.instance.gridMap.GetCircleGridContainType<SoldierModel>(transform.position, m_SoldierModel.nowStatus.weapon.AttackRange + 1);
                newSetModel.Remove(m_SoldierModel);
                float minDistance = m_SoldierModel.nowStatus.weapon.AttackRange + 3;
                if (newSetModel.Count != 0)
                {
                    SoldierModel target = null;
                    foreach (var item in newSetModel)
                    {
                        if (item.nowStatus.nowHp > 0 && item.nowStatus.Belong != m_SoldierModel.nowStatus.Belong && Vector3.Distance(item.transform.position, transform.position) < minDistance)
                        {
                            target = item;
                        }
                    }
                    if (target != null)
                    {
                        targetTo = target.transform.position - transform.position;
                        m_SoldierModel.actionModel.FixFaceTarget(target.transform.position - transform.position);
                        //转向
                    }
                    control.PlayAttack(() =>
                    {
                        //将军杀死最后一个敌人时 执行这个回执函数 会报空的原因是 最后一个敌人死亡之后 直接进入了显示结果界面
                        if (this == null) return;
                        moveSpeed = 1f;
                        //找到没有执行callback的原因
                        //进入arpg模式给 
                    });
                }
            }

        }
        return true;
    }

    public void OnDisable()
    {
        Destroy(this);
    }

    public void Reset()
    {
        control.PlayIdle();
        m_SoldierModel.actionModel.m_Agent.avoidancePriority = 50;
        if (m_SoldierModel.commander != null)
            m_SoldierModel.commander.canSetTarget = true;
        ResetMoveVector();
        CanSetDestantion();
        moveModule.ReturnBack();
    }
    void OnControlTargetHit()
    {


    }
    public void AutoAttack()
    {
        bool isRight = GetComponent<ClickToMove>().IsRight;
        Vector3 vector3 = isRight ? Vector3.right : Vector3.left;
        //更改成使用武器，与构建触发器


        if (targetTo != default)
        {
            MoveToPosition(new Vector2(targetTo.x, targetTo.z), m_Agent.speed / 2 * moveSpeed);
            targetTo = default;
        }
        else
        {
            if (isRight)
                MoveToPosition(Vector2.right, m_Agent.speed / 2 * moveSpeed);
            else
                MoveToPosition(Vector2.left, m_Agent.speed / 2 * moveSpeed);
        }


        var weapon = m_SoldierModel.nowStatus.weapon;
        bool hasAttacked = false;
        GridMapManager.instance.gridMap.GetCircleGridContainTypeWithAction<SoldierModel>(transform.position + vector3 * Auto_Attack_Range, Attack_Area, (model) =>
        {
            if (hasAttacked)
            {
                return;
            }
            //关闭友伤
            //if (!model.Equals(m_SoldierModel) && model.nowStatus.Belong != m_SoldierModel.nowStatus.Belong)
            //{
            //    if (UnitControlManager.instance.GetSoldierSpaceDistance(model, m_SoldierModel) < Auto_Attack_Range)
            //    {
            //        float x = (model.transform.position - transform.position).x;
            //        if (x > 0 && vector3.x > 0)
            //        {
            //            model.nowStatus.BeenMeleeAttack(m_SoldierModel.nowStatus);
            //            hasAttacked = true;
            //        }
            //        else if (x < 0 && vector3.x < 0)
            //        {
            //            model.nowStatus.BeenMeleeAttack(m_SoldierModel.nowStatus);
            //            hasAttacked = true;
            //        }
            //    }
            //}
        });
    }

}
