using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPGManager : Singleton<ARPGManager>
{
    public Dictionary<int, ARPGControl> generals = new Dictionary<int, ARPGControl>();
    public int targetTroopId = -1;
    public ARPGControl currentGeneralControl;
    public InputModule tabTrigger = new ARPGManager_InputModule_Tab();
    public ARPGManager_InputModule inputModule = new ARPGManager_InputModule();
    public SoldierStatus nowTarget;
    public bool canShowSkillUI = true;

    //之后改变
    public bool isActive;

    protected override void Init()
    {

        inputModule.Init();
        tabTrigger.Init();
        //TODO tabTrigger待定
        tabTrigger.Active();
        tabTrigger.RegistInputModuleToSet("Battle");
        inputModule.RegistInputModuleToSet("Battle");
        //TODO:优化Tab的激活意义
        CameraControl.Instance.unlimitedCheck = () => isActive;

        //TODO 需要优化耦合 与战斗场景绑定过深
        BattleManager.instance.mapToBattle += () =>
        {
            tabTrigger.Active();
        };
        BattleManager.instance.battleToMap += () =>
        {
            tabTrigger.Deactive();
        };
        BattleManager.instance.battleToMap += () =>
        {
            Reset();
            ModelReset();
        };

        base.Init();
    }
    /// <summary>
    /// 加载战斗场景时 给ARPGManager设置基础的将军id
    /// </summary>
    /// <param name="targetTroopId"> 加载战场时赋予的默认将军id </param>
    public void SetTargetTroopId(int targetTroopId)
    {
        this.targetTroopId = targetTroopId;
    }

    //TODO 解决在战场上选择将军id的问题
    public bool SetCurrentGeneralControl()
    {
        //初次进入场景 没有选择英雄的话 获取一个英雄id
        if (targetTroopId == -1)
        {
            currentGeneralControl = generals.DictionaryFirst();
            if (currentGeneralControl != null)
            {
                targetTroopId = generals.GetKeysByValue(currentGeneralControl)[0];
            }
            else {
                return false;
            }
        }
        else
        {
            currentGeneralControl = generals[targetTroopId];
        }
        //切换当前PlayerMove

        inputModule.arpgControl = currentGeneralControl;
        //inputModule.originSpeed = inputModule.originSpeed == 0 ? currentGeneralControl.m_Agent.speed : inputModule.originSpeed;
        currentGeneralControl.m_Agent.ResetPath();
        return true;
        //TODO 切换模式时的动画
    }
    public void CloseGuid()
    {

        UnitControlManager.instance.ResetControl();
    }
    public void ModelReset()
    {
        foreach (var pair in generals)
        {
            ARPGControl.RemoveARPGControl(pair.Value.gameObject);
        }
        generals.Clear();
        currentGeneralControl = null;
        inputModule.arpgControl = null;
    }

    /// <summary>
    /// 模式切换
    /// </summary>
    public void ModelSwitch()
    {
        //如果非激活 并且获取到了ARPGcontrol 才激活ARPG模式 如果激活就重置 如果非激活 切无法获取ARPGControl 则退出
        if (!isActive && SetCurrentGeneralControl())
        {
            Active();
        }
        else
        {
            Reset();
        }
    }

    /// <summary>
    /// 模块激活
    /// </summary>
    public void Active()
    {
        
        var target = generals.DictionaryFirst();
        if (target.m_SoldierModel.nowStatus.nowHp <= 0) return;
        if (isActive) return;
        isActive = true;
        //将输入字典转换
        InputManager.Instance.needSwitchToARPG = true;
        //TODO 获取当前选择的将军id 或者默认将军id 激活ARPGInputModle 
        //SetCurrentGeneralControl();
        //激活input
        inputModule.Active();
        //TODO 找到所有的inputModule在激活之后将其添加到对应的聚合当中
        //CameraControl.Instance.ChangeCameraToARPG(currentGeneralControl);
        ////通过ARPGManager中的currentGeneralControl获取当前的transfrom
        //currentGeneralControl.moveModule.TakeOver();
        //UnitControlManager.instance.HideControlUI(currentGeneralControl.m_SoldierModel.nowStatus.commander);
        //currentGeneralControl.control.PlayIdle();
        ////将当前将军的navmesh的priority改变
        //currentGeneralControl.m_SoldierModel.actionModel.m_Agent.avoidancePriority=10;
        //currentGeneralControl.m_SoldierModel.commander.RemoveAttackTarget();
        currentGeneralControl.Active();
        //虽然写在这里 会比写在ARPGSkillListUI中
        //ShowARPGCharacterUI();
        ShowARPGSkillListUI();
        CloseGuid();

        //TODO 角色气泡对话内容的读取

    }



   public void OnControlTargetDie(SoldierStatus soldier)
    {
        //1-直接关闭
        //2-直接GameOver
        //3-切换下一个
        //例子：4-全屏爆炸
        currentGeneralControl.OnControlDie(soldier);
        Reset();

    }

    public void ShowARPGCharacterUI()
    {
        var CharacterUI = UIManager.Instance.GetUI("ARPGCharacterUI") as ARPGCharacterUI;
        nowTarget = currentGeneralControl.m_SoldierModel.nowStatus;
        nowTarget.onDie += OnControlTargetDie;

        if (CharacterUI == null)
            return;
        CharacterUI.Init(currentGeneralControl.m_SoldierModel.nowStatus, currentGeneralControl.general);
        CharacterUI.OnShow();
    }
    public void ShowARPGSkillListUI()
    {
        var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;
        if (SkillListUI == null)
            return;
        SkillListUI.Init(currentGeneralControl.general);
        SkillListUI.OnShow();
    }

    public void HideARPGCharacterUI()
    {
       // nowTarget.onDie -= OnControlTargetDie;
        var CharacterUI = UIManager.Instance.GetUI("ARPGCharacterUI") as ARPGCharacterUI;
        CharacterUI.OnHide();
    }

    public void HideARPGSkillListUI()
    {
        var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;
        SkillListUI.OnHide();


    }
    /// <summary>
    /// 两种情况
    /// </summary>
    public void SwitchGeneral()
    {



    }

    /// <summary>
    /// 数据重置
    /// </summary>
    public void Reset()
    {
        if (!isActive) return;
        isActive = false;
        targetTroopId = -1;
        InputManager.Instance.needSwitchToDefault = true;
        inputModule.Deactive();
        HideARPGSkillListUI();
        //HideARPGCharacterUI();
        CameraControl.Instance.ChangeCameraToBattle();
        UnitControlManager.instance.ShowControlUI();
        if (currentGeneralControl != null)
            currentGeneralControl.Reset();
        currentGeneralControl = null;

        //currentGeneralControl.m_SoldierModel.actionModel.m_Agent.avoidancePriority = 50;
        //currentGeneralControl.control.PlayIdle();
        //currentGeneralControl.ResetMoveVector();
        //currentGeneralControl.CanSetDestantion();
        //currentGeneralControl.moveModule.ReturnBack();


    }
}
