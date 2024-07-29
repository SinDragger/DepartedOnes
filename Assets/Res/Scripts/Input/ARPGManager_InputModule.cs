using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ARPGManager_InputModule : InputModule
{
    public ARPGControl arpgControl;
    public override void Init()
    {
        ClearDic();
        //添加按下按钮的 down
        //角色移动
        keyDic[InputType.PLAYER_UP.ToString()] = new InputResponseNode(InputWDown, 0);
        keyDic[InputType.PLYAER_LEFT.ToString()] = new InputResponseNode(InputADown, 0);
        keyDic[InputType.PLAYER_DOWN.ToString()] = new InputResponseNode(InputSDown, 0);
        keyDic[InputType.PLAYER_RIGHT.ToString()] = new InputResponseNode(InputDDown, 0);
        //角色技能
        keyDic[InputType.PLAYER_SKILL1.ToString()] = new InputResponseNode(OnSkillDown1, 0);
        keyDic[InputType.PLAYER_SKILL2.ToString()] = new InputResponseNode(OnSkillDown2, 0);
        keyDic[InputType.PLAYER_SKILL3.ToString()] = new InputResponseNode(OnSkillDown3, 0);
        keyDic[InputType.PLAYER_ULTIMATE.ToString()] = new InputResponseNode(OnSkillDown4, 0);

        //鼠标左键设置为攻击键 平a
        //鼠标右键是交互键 TODO 交互的类别 交互的方式 方法
        //mouseDic[MouseCode.LEFT_DOWN] = new InputResponseNode(AttackTarget, 5);
        mouseDic[MouseCode.LEFT_UP] = new InputResponseNode(InputToDoNothing, 5);
        //mouseDic[MouseCode.LEFT] = new InputResponseNode(AttackTarget, 5);

        mouseDic[MouseCode.RIGHT_UP] = new InputResponseNode(InputToDoNothing, 5);
        mouseDic[MouseCode.RIGHT] = new InputResponseNode(InputToDoNothing, 5);
        mouseDic[MouseCode.MIDDLE_DOWN] = new InputResponseNode(InputToDoNothing, 5);
        mouseDic[MouseCode.SCROLL] = new InputResponseNode(InputToDoNothing, 1);
        mouseDic[MouseCode.MIDDLE_DOWN] = new InputResponseNode(InputToDoNothing, 1);
        mouseDic[MouseCode.MIDDLE] = new InputResponseNode(InputToDoNothing, 1);
        base.Init();
    }
    bool OnSkillDown1(object param)
    {
        arpgControl.characterSkillManager.OnSkillDown1();
        return true;
    }
    bool OnSkillDown2(object param)
    {
        arpgControl.characterSkillManager.OnSkillDown2();
        return true;
    }
    bool OnSkillDown3(object param)
    {
        arpgControl.characterSkillManager.OnSkillDown3();
        return true;
    }
    bool OnSkillDown4(object param)
    {
        arpgControl.characterSkillManager.OnSkillDown4();
        return true;
    }
    bool InputWDown(object param)
    {
        arpgControl.WDown();
        return true;
    }
    bool InputADown(object param)
    {
        arpgControl.ADown();
        return true;
    }
    bool InputSDown(object param)
    {
        arpgControl.SDown();
        return true;
    }
    bool InputDDown(object param)
    {
        arpgControl.DDown();
        return true;
    }

    /// <summary>
    /// TODO:优化进战斗底层
    /// </summary>
    //bool AttackTarget(object param)
    //{
    //    return arpgControl.AttackTarget(param);
    //}
    public override void Deactive()
    {
        base.Deactive();
    }
}
