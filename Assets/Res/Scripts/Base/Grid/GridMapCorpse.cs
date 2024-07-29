using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapCorpse : GridMapItem
{
    public SoldierStatus soldier;
    public bool isRaising;
    public void Init(SoldierStatus soldier)
    {
        this.soldier = soldier;
        pos = soldier.model.lastPosition;
        occupyMapGrids = soldier.model.occupyMapGrids;
    }

    public override void RemoveGridOccupy()
    {
        //把自己变成一具尸体进行Item的存储
        foreach (var grid in occupyMapGrids)
        {
            grid.GetGridContain<GridMapCorpse>().Remove(this);
        }
    }
}
