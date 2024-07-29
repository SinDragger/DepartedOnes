using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapManager : MonoSingleton<GridMapManager>, IBattleManageSingleton
{
    public int priority => 10;
    public static float gridLength = 2f;
    public int gridXMax = 1;
    public int gridYMax = 1;
    /// <summary>
    /// 落后逻辑 已废弃
    /// </summary>
    public GridMap<SoldierModel> unitGridMap;
    /// <summary>
    /// 网格地图
    /// </summary>
    public GridMap gridMap;
    Vector2 center;
    public void Init(System.Action callback = null)
    {
        center = new Vector2(gridLength * (float)gridXMax / 2, gridLength * (float)gridYMax / 2);
        //unitGridMap = new GridMap<SoldierModel>();
        //unitGridMap.Init(gridXMax, gridYMax, gridLength);
        gridMap = new GridMap();
        gridMap.Init(gridXMax, gridYMax, gridLength);
    }
    public void OnActive()
    {
        if (gridMap == null)
        {
            gridMap = new GridMap();
            gridMap.Init(gridXMax, gridYMax, gridLength);
        }
        else if (gridMap.gridXMax != gridXMax || gridMap.gridYMax != gridYMax)
        {
            center = new Vector2(gridLength * (float)gridXMax / 2, gridLength * (float)gridYMax / 2);
            gridMap = new GridMap();
            gridMap.Init(gridXMax, gridYMax, gridLength);
        }
    }

    public void OnDeactive()
    {
        //if (gridMap == null) return;
        //foreach (var grid in gridMap.map)
        //{
        //    grid.ResetData();
        //}
    }

    public void ResetData()
    {
        if (gridMap == null) return;
        foreach (var grid in gridMap.map)
        {
            grid.ResetData();
        }
    }

    //对一个范围的grid进行获取——演变成选取全屏
    public void SelectAllTargetArea(Vector2 aPoint, Vector2 bPoint, out List<ControlFlag> outr)
    {
        //1:延展成四个点
        float leftX, rightX, upY, downY;
        if (aPoint.x < bPoint.x)
        {
            leftX = aPoint.x;
            rightX = bPoint.x;
        }
        else
        {
            leftX = bPoint.x;
            rightX = aPoint.x;
        }

        if (aPoint.y < bPoint.y)
        {
            upY = bPoint.y;
            downY = aPoint.y;
        }
        else
        {
            upY = aPoint.y;
            downY = bPoint.y;
        }
        Vector2[] points = {
            new Vector2(leftX,upY),
            new Vector2(rightX,upY),
            new Vector2(rightX,downY),
            new Vector2(leftX,downY)
        };
        var grids = gridMap.GetGridsInScreenPoints(points);
        HashSet<CommandUnit> result = new HashSet<CommandUnit>();
        //根据可能的索引模式进行匹配？
        //默认搜索单位
        foreach (var grid in grids)
        {
            foreach (var soldier in grid.GetGridContain<SoldierModel>())
            {
                result.Add(((SoldierModel)soldier).commander);
            }
        }
        outr = UnitControlManager.instance.AddMulCommands(result);
    }



    public void OnUpdate()
    {
    }

#if UNITY_EDITOR
    public float gridGizmosHeight;
    public int gridGizmosNum;
    BaseGrid<SoldierModel>[,] soldierGizmosGridMap;
    HashSet<BaseGrid<SoldierModel>> targetPositionClick = new HashSet<BaseGrid<SoldierModel>>();
    public (int, int) TransferVectorToArrayFlag(Vector3 worldPosition)
    {
        (int, int) result = (Mathf.FloorToInt((worldPosition.x + center.x) / gridLength), Mathf.FloorToInt((worldPosition.z + center.y) / gridLength));
        //-0.5变成长度除以width 
        return result;
    }

    void OnDrawGizmos()
    {
        if (unitGridMap != null && unitGridMap.map != null)
        {
            soldierGizmosGridMap = unitGridMap.map;
        }
        if (soldierGizmosGridMap == null)
        {
            soldierGizmosGridMap = new BaseGrid<SoldierModel>[gridXMax, gridYMax];//TODO:由地图生成后进行
            for (int i = 0; i < gridXMax; i++)
            {
                for (int j = 0; j < gridYMax; j++)
                {
                    soldierGizmosGridMap[i, j] = new BaseGrid<SoldierModel>();
                    soldierGizmosGridMap[i, j].x = i;
                    soldierGizmosGridMap[i, j].y = j;
                }
            }
            return;
        }
        if (soldierGizmosGridMap.Length != gridXMax * gridYMax)
        {
            soldierGizmosGridMap = null;
            return;
        }
        foreach (var grid in soldierGizmosGridMap)
        {
            if (targetPositionClick != null && targetPositionClick.Contains(grid))
            {

                Gizmos.color = Color.red;
                PositionDraw(grid, GridPosition.SOUTHWEST, GridPosition.NORTHWEST, 0.1f);
                PositionDraw(grid, GridPosition.NORTHWEST, GridPosition.NORTHEAST, 0.1f);
                PositionDraw(grid, GridPosition.NORTHEAST, GridPosition.SOUTHEAST, 0.1f);
                PositionDraw(grid, GridPosition.SOUTHEAST, GridPosition.SOUTHWEST, 0.1f);
            }
            else
            {
                Gizmos.color = Color.green;
                PositionDraw(grid, GridPosition.SOUTHWEST, GridPosition.NORTHWEST);
                PositionDraw(grid, GridPosition.NORTHWEST, GridPosition.NORTHEAST);
                PositionDraw(grid, GridPosition.NORTHEAST, GridPosition.SOUTHEAST);
                PositionDraw(grid, GridPosition.SOUTHEAST, GridPosition.SOUTHWEST);
            }
            //grid.GetPositionPoint(GridPosition.SOUTHWEST);
            //Gizmos.DrawLine(CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.SOUTHWEST) - new Vector2(gridLength * gridXMax/2, gridLength * 50), gridGizmosHeight), CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.NORTHWEST) - new Vector2(gridLength * 50, gridLength * 50), gridGizmosHeight));
            //Gizmos.DrawLine(CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.NORTHWEST) - new Vector2(gridLength * gridXMax / 2, gridLength * 50), gridGizmosHeight), CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.NORTHEAST) - new Vector2(gridLength * 50, gridLength * 50), gridGizmosHeight));
            //Gizmos.DrawLine(CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.NORTHEAST) - new Vector2(gridLength * gridXMax / 2, gridLength * 50), gridGizmosHeight), CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.SOUTHEAST) - new Vector2(gridLength * 50, gridLength * 50), gridGizmosHeight));
            //Gizmos.DrawLine(CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.SOUTHEAST) - new Vector2(gridLength * gridXMax / 2, gridLength * 50), gridGizmosHeight), CastUtil.V2TV3(grid.GetPositionPoint(GridPosition.SOUTHWEST) - new Vector2(gridLength * 50, gridLength * 50), gridGizmosHeight));
        }
    }
    void PositionDraw(BaseGrid<SoldierModel> grid, GridPosition start, GridPosition end, float heightDelta = 0f)
    {
        if (center == default)
            center = new Vector2(gridLength * (float)gridXMax / 2, gridLength * (float)gridYMax / 2);
        Gizmos.DrawLine(CastUtil.V2TV3(grid.GetPositionPoint(start) - center, gridGizmosHeight + heightDelta), CastUtil.V2TV3(grid.GetPositionPoint(end) - center, gridGizmosHeight + heightDelta));
    }

#endif
}

public class GridMap<T> where T : Object
{
    float gridLength;
    public BaseGrid<T>[,] map;
    int gridXMax;
    int gridYMax;
    Vector2 center;
    public void Init(int xMax, int yMax, float gridLength)
    {
        gridXMax = xMax;
        gridYMax = yMax;
        this.gridLength = gridLength;
        map = new BaseGrid<T>[gridXMax, gridYMax];//TODO:由地图生成后进行
        for (int i = 0; i < gridXMax; i++)
        {
            for (int j = 0; j < gridYMax; j++)
            {
                map[i, j] = new BaseGrid<T>();
                map[i, j].x = i;
                map[i, j].y = j;
            }
        }
        center = new Vector2(gridLength * gridXMax / 2, gridLength * gridYMax / 2);
    }
    //增删查改

}

public class BaseGrid<T> where T : Object
{
    public int x, y;
    public List<T> storage = new List<T>();

    /// <summary>
    /// 方格序列格子内的位置
    /// </summary>
    public Vector2 GetPositionPoint(GridPosition postionType)
    {
        switch (postionType)
        {
            case GridPosition.EAST:
                return new Vector2(GridMapManager.gridLength * (x + 1f), GridMapManager.gridLength * (y + 0.5f));
            case GridPosition.WEST:
                return new Vector2(GridMapManager.gridLength * x, GridMapManager.gridLength * (y + 0.5f));
            case GridPosition.NORTH:
                return new Vector2(GridMapManager.gridLength * (x + 0.5f), GridMapManager.gridLength * (y + 1f));
            case GridPosition.SOUTH:
                return new Vector2(GridMapManager.gridLength * (x + 0.5f), GridMapManager.gridLength * (y + 0f));
            case GridPosition.SOUTHWEST:
                return new Vector2(GridMapManager.gridLength * x, GridMapManager.gridLength * y);
            case GridPosition.NORTHWEST:
                return new Vector2(GridMapManager.gridLength * x, GridMapManager.gridLength * (y + 1f));
            case GridPosition.NORTHEAST:
                return new Vector2(GridMapManager.gridLength * (x + 1f), GridMapManager.gridLength * (y + 1f));
            case GridPosition.SOUTHEAST:
                return new Vector2(GridMapManager.gridLength * (x + 1f), GridMapManager.gridLength * y);
        }
        return new Vector2(GridMapManager.gridLength * (x + 0.5f), GridMapManager.gridLength * (y + 0.5f));
    }
}

public enum GridPosition
{
    CENTER,//中心·
    EAST,//东→
    WEST,//西←
    NORTH,//北↑
    SOUTH,//南↓
    SOUTHWEST,//西南↙ 
    NORTHWEST,//西北↖
    NORTHEAST,//东北↗
    SOUTHEAST,//东南↘
}