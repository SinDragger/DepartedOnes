public static class Constant_RogueData
{
    /// <summary> 军队疲劳度 </summary>
    public const string LegionRes_Fatigue = "RogueRes_Fatigue";
    /// <summary> 军队补给品 </summary>
    public const string LegionRes_Supplies = "RogueRes_Supplies";


    /// <summary> 遗物id </summary>
    public const string RogueItem = "RogueRes_Item";


    public const string SelectionUIButton = "RogueBehaviorBetton";
}

public enum UIBehavior
{
    /// <summary> 隐藏ui，一般用于战斗选项 </summary>
    HideUI = 0,
    /// <summary> 直接前往下一个节点 </summary>
    ToNextNode = 1,
    /// <summary> 结束当前事件 </summary>
    EndEvent = 2,
}