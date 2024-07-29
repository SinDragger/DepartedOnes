using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网格地图上的物体
/// </summary>
public class GridMapItem
{
    public Vector3 pos;
    float size;
    public HashSet<BaseGrid> occupyMapGrids;
    public virtual void UpdateGridOccupy(Vector3 worldPosition,float occupySize)
    {
        HashSet<BaseGrid> mapGrids = GridMapManager.instance.gridMap.GetGridsInCircle(worldPosition, occupySize);
        //将自己点和坐标传入以获取对应
        occupyMapGrids.RemoveWhere((a) =>
        {
            if (!mapGrids.Contains(a))
            {
                a.GetGridContain<GridMapItem>().Remove(this);
                return true;
            }
            return false;
        });
        foreach (var grid in mapGrids)
        {
            if (occupyMapGrids.Add(grid))//增加成功
            {
                grid.GetGridContain<GridMapItem>().Add(this);
            }
        }
    }

    public virtual void RemoveGridOccupy()
    {
        //把自己变成一具尸体进行Item的存储
        foreach (var grid in occupyMapGrids)
        {
            grid.GetGridContain<GridMapItem>().Remove(this);
        }
    }
}
