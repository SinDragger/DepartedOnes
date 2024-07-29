using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_EmojiBox : StoryScriptPlayerAction
{
    
    public override string processHead => "EmojiBox";

    /// <summary>
    /// 0 
    /// 1 EmojiName
    /// 2 ID,ID,ID
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {
        //传入的第二个数组元素是GameObjectID 和emotion的名字通过,分割
        string[] characters = paramLine[2].Split(',');
        foreach (var c in characters)
        {
        var character = StoryManager.instance.GetCharacter(c);
        var gameObject = EmotionBubbleManager.Instance.ShowEmotionBubble(character.refData.model.transform, paramLine[1]);

        }
        //TODO emotionParams[0] 在CharacterManager中去拿到对应ID  Character获取到transform传入成为target

    }
   
}
