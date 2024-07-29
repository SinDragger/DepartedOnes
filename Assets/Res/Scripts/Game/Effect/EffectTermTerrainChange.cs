using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTermTerrainChange : EffectTerm
{
    /// <summary>
    /// 效果预制体名称
    /// </summary>
    public int layer;
    public int range;
    public override void Execution()
    {
        var terrain = BattleManager.instance.nowBattleMap.mapterrain;
        TerrainUtil.ChangeTexture(terrain, effectPos, range);
        BattleManager.instance.UpdateMiniMap();
    }
}
