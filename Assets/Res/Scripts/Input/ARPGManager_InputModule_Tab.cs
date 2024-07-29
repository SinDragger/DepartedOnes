using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPGManager_InputModule_Tab : InputModule
{
    public override void Init()
    {
        //切换模式 记得删除 对应的高级指令
        //keyUpDic[InputType.TAB.ToString()] = new InputResponseNode(InputTab, 0);
    }

    /// <summary>
    /// 切换回战场默认输入模式离开ARPG输入模式
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    bool InputTab(object param)
    {
        if (BattleManager.instance.isActive && ARPGManager.Instance.generals.Count > 0)
        {
            ARPGManager.Instance.ModelSwitch();
        }
        else
        {
            Debug.LogError(ARPGManager.Instance.generals.Count);
        }
        //切换时将InputManager中的4个按键InputResponseChain<>中的 ARPG相关的按键的Func删除 
        return true;
    }

    public override void Active()
    {
        base.Active();
        //显示TabUI
    }
    public override void Deactive()
    {
        base.Deactive();
        //关闭TabUI
    }
}
