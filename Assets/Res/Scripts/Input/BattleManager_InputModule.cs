using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class BattleManager_InputModule : InputModule
{
    //public float refreshTiem = 0;
    //public List<ControlFlag> controlFlagList = new List<ControlFlag>();
    //public List<ControlFlag> prepareControlFlagList = new List<ControlFlag>();

    public override void Init()
    {
        mouseDic[MouseCode.LEFT_UP] = new InputResponseNode(InputLeftUp, 0);
        mouseDic[MouseCode.LEFT] = new InputResponseNode(InputLeftCheck, 0);

        mouseDic[MouseCode.RIGHT] = new InputResponseNode(InputRightCheck, 2);
        mouseDic[MouseCode.RIGHT_UP] = new InputResponseNode(InputRightUp, 2);
        mouseDic[MouseCode.RIGHT_DOWN] = new InputResponseNode(InputRightDown, 2);

        keyDic[InputType.Formation1.ToString()] = new InputResponseNode((param) => { FormationControl(0); return true; }, 0);
        keyDic[InputType.Formation2.ToString()] = new InputResponseNode((param) => { FormationControl(1); return true; }, 0);
        keyDic[InputType.Formation3.ToString()] = new InputResponseNode((param) => { FormationControl(2); return true; }, 0);
        keyDic[InputType.Formation4.ToString()] = new InputResponseNode((param) => { FormationControl(3); return true; }, 0);
        keyDownDic[InputType.SELECT_ALL_ABLE_TROOP.ToString()] = new InputResponseNode(InputAllSelect, 0);

        //类似如此？

        base.Init();
    }
    public override void Active()
    {
        base.Active();
        rightDownTime = 0;
    }

    bool InputLeftUp(object param)
    {
        if (InputManager.Instance.inputMode == InputPointType.NONE)
        {
            if (!EventSystem.current.IsPointerOverGameObject(-1))
            {
                BattleManager.instance.ActiveClick((Vector2)param);//对游戏地图的点击
                return true;
            }
            else
            {
                var control = InputManager.Instance.NowRaycastFind<ControlFlag>();//TODO:做平台的分离
                if (control != null)//可攻击
                {
                    if (!control.commander.belong.Equals(BattleManager.instance.controlBelong)) return false;
                    UnitControlManager.instance.TargetCommondModelClick(control.commander);
                    return true;
                    //指定攻击
                }
            }
        }
        else if (InputManager.Instance.inputMode == InputPointType.UI)
        {
        }
        return false;
    }

    bool InputLeftCheck(object param)
    {
        if (InputManager.Instance.inputMode == InputPointType.NONE && InputManager.Instance.mouseLeftDeltaDistance > 30)
        {
            //30帧范围没有做别的则进入框选
            //激活另一个InputModule 优先级为3 包含了 可以有不紧急esc 和右键所有内容
            UIManager.Instance.ActiveSelectBox();
            InputManager.Instance.SetInputMode(InputPointType.TARGET_CHOOSE);

            return true;

        }

        if (InputManager.Instance.inputMode == InputPointType.TARGET_CHOOSE)
        {
            UnitControlManager.instance.frameSelectInputModule.Active();
            return true;
        }

        return false;
    }


    float rightDownTime;

    bool InputRightDown(object param)
    {
        rightDownTime = 0;

        return false;
    }

    bool InputRightCheck(object param)
    {
        rightDownTime += Time.deltaTime;
        if (rightDownTime + Time.deltaTime > 0.5f || InputManager.Instance.mouseRightDeltaDistance > 33f)
        {
            UnitControlManager.instance.formationInputModule.Active();
            return true;
        }
        return false;
    }

    public override void Deactive()
    {
        base.Deactive();
        UnitControlManager.instance.formationInputModule.Deactive();
    }

    bool InputRightUp(object param)
    {
        Vector2 mousePos = (Vector2)param;
        if (InputManager.Instance.inputMode == InputPointType.NONE)
        {
            var control = InputManager.Instance.NowRaycastFind<ControlFlag>();//TODO:做平台的分离
            if (control != null)//可攻击
            {
                if (UnitControlManager.instance.ChooseToAttack(control.commander))
                {
                    return true;
                }
                //指定攻击
            }
            BattleManager.instance.ActiveClick(InputManager.Instance.rightFirstV + (mousePos - InputManager.Instance.rightFirstV) / 2, 1);//可能是移动、进入、占领、摧毁、收集、触发、晋升等交互。向外委托
            return true;
        }

        return false;
    }

    bool InputAllSelect(object param)
    {
        if (InputManager.Instance.inputMode == InputPointType.NONE)
        {
            UnitControlManager.instance.ChooseAllAble();//指定攻击
            return true;
        }
        return false;
    }

    /// <summary>
    /// 编队相关的控制
    /// </summary>
    /// <param name="flag"></param>
    void FormationControl(int flag)
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftControl) && InputManager.Instance.isShiftMulAble)
        {
            UnitControlManager.instance.SetFormation(flag);
            return;
        }
#else
        if (Input.GetKey(KeyCode.LeftControl))
        {
            
            UnitControlManager.instance.SetFormation(flag);
            return;
        }
#endif
        UnitControlManager.instance.UseFormation(flag);
    }

}
