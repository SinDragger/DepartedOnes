using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTermRaiseCorpse : EffectTerm
{
    public float range;
    public int maxNumber;
    public override void Execution()
    {
        HashSet<GridMapCorpse> array = GridMapManager.instance.gridMap.GetCircleGridContainType<GridMapCorpse>(effectPos, range);
        var willRaiseCorpse = array.GetHashSetItems(maxNumber, (c) =>
        {
            return c.soldier.EntityData.speciesType != "Beast";
        });
        ObjectPoolManager.Instance.Recycle(array);
        List<SoldierStatus> willSeperateList = new List<SoldierStatus>();
        //构筑一个虚拟的新的Command
        foreach (var corpse in willRaiseCorpse)
        {
            if (corpse.isRaising) continue;
            if (corpse.soldier.commander.belong == BattleManager.instance.controlBelong)
            {
                int soulPoint = GameManager.instance.playerData.soulPoint;
                if (soulPoint >= 5)
                {
                    GameManager.instance.playerData.soulPoint -= 5;
                    corpse.soldier.commander.RaiseUnit(corpse.soldier);
                    corpse.isRaising = true;
                }
                else
                {
                    break;
                }

            }
            else
            {

                //不复活敌军
                continue;
                corpse.soldier.commander.RaiseAndSeperateSoldier(corpse.soldier, "Zombie");
                willSeperateList.Add(corpse.soldier);
            }
        }
        if (willSeperateList.Count > 0)
        {
            UnitControlManager.instance.RaiseToTempCommand(willSeperateList, BattleManager.instance.controlBelong);
        }
    }
}
