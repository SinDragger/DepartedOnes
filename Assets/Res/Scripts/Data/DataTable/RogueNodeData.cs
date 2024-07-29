using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueNodeData : AggregationEntity
{
    /// <summary> 前置节点 </summary>
    public string preNodeName;
    /// <summary> 后续节点 </summary>
    public string nextNodeName;

    public string nodeName;
    /// <summary> 节点故事描述 </summary>
    public string storyDescribe;
    /// <summary> 节点选择行为 </summary>
    public RogueNodeBehavior[] behavior;
    /// <summary> 节点选择数据和行为 </summary>
    public RogueNodeSelectionData[] selections;

}
