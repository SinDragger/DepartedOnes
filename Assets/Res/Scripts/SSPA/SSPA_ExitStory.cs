using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_ExitStory : StoryScriptPlayerAction
{
    public override string processHead => "ExitStory";


    public override void ProcessParamLine(string[] paramLine)
    {
        Debug.Log("ExitStory  退出故事");
        StoryManager.instance.storyPlayer.nowStory = null;
        StoryManager.instance.Exit();
    }

}
