using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class StoryScriptPlayer
{


   public StoryScript nowStory = null;

    public StoryScriptPlayerAction nowAction = new StoryScriptPlayerAction();

    public Dictionary<string, StoryScriptPlayerAction> storyActionDic = new Dictionary<string, StoryScriptPlayerAction>();




    public StoryScriptPlayer()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type baseType = typeof(StoryScriptPlayerAction);

        var derivedTypes = assembly.GetTypes()
            .Where(type => baseType.IsAssignableFrom(type) && type != baseType);

        foreach (Type derivedType in derivedTypes)
        {
            var createdInstance = Activator.CreateInstance(derivedType);
            if (createdInstance is StoryScriptPlayerAction)
            {
                StoryScriptPlayerAction processAction = (createdInstance as StoryScriptPlayerAction);
                storyActionDic.Add(processAction.processHead, processAction);
                //Debug.LogError(processAction.processHead);
            }
        }
    }
    public void StartPlaying(StoryScript story)
    {
        //开始读取脚本内容 
        nowStory = story;
        //  ProcessStoryAciton();
    }

    public void ProcessStoryAciton()
    {
        //判断nowAction的ComfirmBlockEnd是否完成
        if (nowStory == null) return;
        //sb 老是在这里卡一下
        if (nowAction == null || !nowAction.ComfirmBlockEnd()) return;
        if (nowStory.isEnd)
        {
            nowStory = null;
            StoryManager.instance.Exit();
            return;
            //通知关闭剧情模式 nowStory=null;
        }
        string[] data = nowStory.UpdateStory();
        nowAction = storyActionDic[data[0]];
        nowAction.ProcessParamLine(data);
        ProcessStoryAciton();
    }





}
