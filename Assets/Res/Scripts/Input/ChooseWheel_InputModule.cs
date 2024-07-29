using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseWheel_InputModule : InputModule
{

    public override void Init()
    {
        mouseDic[MouseCode.LEFT_UP] = new InputResponseNode(InputLeftUp, 3);
        mouseDic[MouseCode.LEFT_DOWN] = new InputResponseNode(InputLeftDown, 3);
        mouseDic[MouseCode.LEFT] = new InputResponseNode(InputLeftCheck, 3);

        mouseDic[MouseCode.RIGHT_DOWN] = new InputResponseNode(InputRightDown, 3);
        mouseDic[MouseCode.RIGHT] = new InputResponseNode(InputRight, 3);
        mouseDic[MouseCode.RIGHT_UP] = new InputResponseNode(InputRightUp, 3);
        keyUpDic[InputType.ESC.ToString()] = new InputResponseNode(InputESC, 3);

        base.Init();
    }
    bool InputLeftUp(object param)
    {
        //交给Button实现了
        Deactive();
        return true;
    }
    bool InputLeftDown(object param)
    {
        //在技能轮外关闭技能轮
        CloseChooseWheel();
       
        return true;
    }
    bool InputLeftCheck(object param)
    {

        return true;
    }
    bool InputESC(object param)
    {
        //关闭技能轮
        BattleCastManager.instance.CloseChooseWheel();
        return true;
    }
    bool InputRightDown(object param)
    {

        //在技能Button上显示技能细节
        //在技能轮外关闭技能轮

        CloseChooseWheel();
        return true;
    }
    void CloseChooseWheel()
    {
        

        Vector3 screenCenterPos = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
        Vector3 targetDelta = screenCenterPos - BattleCastManager.instance.chooseWheel.transform.localPosition;
        if (Vector3.Distance(Vector3.zero, targetDelta) > 120)
        {
            //关闭技能轮 切换模式
            BattleCastManager.instance.CloseChooseWheel();
        }
    }
    bool InputRight(object param)
    {
        //TODO
        
        //RinghtFirstV与 Input.mousePosition 做方向向量

        //再把这个方向向量传入到ChooseWheel中返回一个预选的技能


        return true;
    }

    bool InputRightUp(object param)
    {
        if (!BattleCastManager.instance.chooseWheel.gameObject.activeSelf) { 
            BattleCastManager.instance.CloseChooseWheel();
            return true;
        }
        //判断当前鼠标位置超过技能轮没有 
        Vector3 screenCenterPos = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
        Vector3 targetDelta = screenCenterPos - BattleCastManager.instance.chooseWheel.transform.localPosition;
        if (Vector3.Distance(Vector3.zero, targetDelta) > 30)
        {
            BattleCastManager.instance.chooseWheel.TryPosTrigger(targetDelta);
        }
        //有预选技能就直接进入预选技能的施法界面 
        //没有预选技能不做任何处理

        return true;
    }


}
