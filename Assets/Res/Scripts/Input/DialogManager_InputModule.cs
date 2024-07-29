using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager_InputModule : InputModule
{

    public override void Init()
    {
        ClearDic();
        //添加按下按钮的 down

        keyDownDic[InputType.Skip.ToString()] = new InputResponseNode(OnSpaceDown, 2);
        mouseDic[MouseCode.LEFT_DOWN] = new InputResponseNode(OnSpaceDown, 2);
#if UNITY_EDITOR
        keyDownDic[InputType.ESC.ToString()] = new InputResponseNode(OnESCDown, 0);
#endif
        //松开按钮的 up



        //鼠标右键是交互键 TODO 交互的类别 交互的方式 方法
        mouseDic[MouseCode.LEFT_UP] = new InputResponseNode(InputToDoNothing, 1);
        mouseDic[MouseCode.LEFT] = new InputResponseNode(InputToDoNothing, 1);
        mouseDic[MouseCode.RIGHT_UP] = new InputResponseNode(InputToDoNothing, 1);
        base.Init();
    }

    bool OnSpaceDown(object param)
    {


        if (!DialogManager.instance.boxUI.completeNowDialogue)
        {

            //快进掉当前对话 直接显示对话内容 
            DialogManager.instance.SkipDialogue();
            //直接替换当前内容
            return true;

        }
        //对话内容显示完成等待玩家点击
        DialogManager.instance.IsOnClicked();



        return true;
    }

    bool OnESCDown(object param)
    {


        DialogManager.instance.boxUI.StopAllCoroutines();

        //TODO 着急在这里做 当对话完之后等待剧情演出的脚本看其中是否还有需要进行的内容
        string[] myParams = { "ExitStory" };
        // StoryManager.instance.ExitStory(myParams);

        return true;
    }
}
