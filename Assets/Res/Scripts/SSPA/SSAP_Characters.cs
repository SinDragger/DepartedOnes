using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_Characters : StoryScriptPlayerAction
{

    public override string processHead => "Characters";


    /// <summary>
    /// Characters (ID1:模型1:势力),(ID2:模型2:势力),…,(IDN:模型N:势力)
    /// </summary>
    /// <param name="paramLine"></param>
    public override void ProcessParamLine(string[] paramLine)
    {
        //TODO 先切 paramLine[1].Split(','); 将每个角色的元组数据切下来(ID1:模型1:势力)数组
        var characters = paramLine[1].Split(',');
        List<(string, string, string)> castMemberList = new List<(string, string, string)>();
        foreach (string segment in characters)
        {
            int startIndex = segment.IndexOf("(");
            int endIndex = segment.IndexOf(")");

            if (startIndex != -1 && endIndex != -1)
            {
                string result = segment.Substring(startIndex + 1, endIndex - startIndex - 1);
                var tupleArray = result.Split(':');
                (string, string, string) tuple = (tupleArray[0], tupleArray[1], tupleArray[2]);
                castMemberList.Add(tuple);
            }
        }
        StoryManager.instance.nowScript.castMemberList.AddRange(castMemberList);

    }


}
