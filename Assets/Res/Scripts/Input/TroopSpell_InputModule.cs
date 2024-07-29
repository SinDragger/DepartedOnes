using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopSpell_InputModule : InputModule
{

    public override void Init()
    {
        //LeftD 确定位置   isCastingSpell=true;
        //Left 增加施法次数 达到最大施法情况 
        //LeftUp 施法 Deactive();

        //RightD 取消施法
        //RightUp 关闭自生Module Deactive();
        //ESC 取消施法 关闭自生Module Deactive();
        mouseDic[MouseCode.LEFT_UP] = new InputResponseNode(InputLeftUp, 3);
        mouseDic[MouseCode.LEFT_DOWN] = new InputResponseNode(InputLeftDown, 3);
        mouseDic[MouseCode.LEFT] = new InputResponseNode(InputLeft, 3);

        mouseDic[MouseCode.RIGHT_DOWN] = new InputResponseNode(InputRightDown, 3);
        mouseDic[MouseCode.RIGHT] = new InputResponseNode(InputRight, 3);
        mouseDic[MouseCode.RIGHT_UP] = new InputResponseNode(InputRightUp, 3);


        base.Init();
    }
    public override void Active()
    {
        base.Active();
    }
    public override void Deactive()
    {
        base.Deactive();
        BattleCastManager.instance.CompleteReset();
    }

    bool InputLeftDown(object param) {
        //确定位置不做改变
        BattleCastManager.instance.determinePos = InputManager.Instance.mouseWorldPos;
        BattleCastManager.instance.determineTransfrom = true;
        return true;
    }

    bool InputLeft(object param)
    {
        if (!BattleCastManager.instance.determineTransfrom) {
            return true;
        }
        BattleCastManager.instance.determinePos = InputManager.Instance.mouseWorldPos;
        BattleCastManager.instance.determineTransfrom = true;
        BattleCastManager.instance.StartContinousSpeelCast();
        //拖拽更改施法逻辑
        return true;//TODO尝试触发当前选中施法轮盘的内容
        
    }
    bool InputLeftUp(object param)
    {
        if (!BattleCastManager.instance.determineTransfrom)
        {
            return true;
        }
        //调用释放技能的方法 然后关闭自己
        BattleCastManager.instance.StartSpeelCast();
        return true;
    }
    bool InputRightDown(object param)
    {
        //确定位置不做改变
        BattleCastManager.instance.indicate.gameObject.SetActive(false);
        return true;
    }
    bool InputRight(object param)
    {
        //确定位置不做改变
      
        return true;
    }
    bool InputRightUp(object param)
    {
        //确定位置不做改变
        Deactive();
        return true;
    }
}
