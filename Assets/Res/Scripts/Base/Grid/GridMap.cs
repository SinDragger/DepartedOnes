using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridMap
{
    float gridLength;
    public BaseGrid[,] map;
    public int gridXMax;
    public int gridYMax;
    Vector2 center;
    public void Init(int xMax, int yMax, float gridLength)
    {
        gridXMax = xMax;
        gridYMax = yMax;
        this.gridLength = gridLength;
        map = new BaseGrid[gridXMax, gridYMax];//TODO:由地图生成后进行
        for (int i = 0; i < gridXMax; i++)
        {
            for (int j = 0; j < gridYMax; j++)
            {
                map[i, j] = new BaseGrid();
                map[i, j].x = i;
                map[i, j].y = j;
            }
        }
        center = new Vector2(gridLength * gridXMax / 2, gridLength * gridYMax / 2);
    }
    //增删查改

    public HashSet<object> GetTargetGridContain<T>(Vector3 worldPosition)
    {
        return GetGrid(worldPosition).GetGridContain<T>();
    }

    HashSet<BaseGrid> temp = new HashSet<BaseGrid>();

    public void GridRegist<T>(Vector3 pos, float radius,T self, ref HashSet<BaseGrid> outSet,System.Action<BaseGrid> newGridAction = null)
    {
        HashSet<BaseGrid> mapGrids = GridMapManager.instance.gridMap.GetGridsInCircle(pos, radius);
        temp.Clear();
        ////将自己点和坐标传入以获取对应
        foreach (var grid in outSet)
        {
            if (!mapGrids.Remove(grid))//新有 旧无
            {
                grid.GetGridContain<T>().Remove(self);
                temp.Add(grid);
            }
        }
        foreach (var grid in temp)
        {
            outSet.Remove(grid);
        }
        //残留的新的
        foreach (var grid in mapGrids)
        {
            outSet.Add(grid);
            //nowStatus.commander.OcupyGrid(grid);
            newGridAction?.Invoke(grid);
            grid.GetGridContain<T>().Add(self);
        }
    }

    public HashSet<object> GetCircleGridContain<T>(Vector3 worldPosition, float radius)
    {
        HashSet<object> result = ObjectPoolManager.Instance.Spawn<HashSet<object>>();
        result.Clear();
        var array = GetGridsInCircle(worldPosition, radius);
        foreach (var grid in array)
        {
            result.UnionWith(grid.GetGridContain<T>());
        }
        return result;
    }

    public HashSet<T> GetCircleGridContainType<T>(Vector3 worldPosition, float radius)
    {
        HashSet<T> result = ObjectPoolManager.Instance.Spawn<HashSet<T>>();
        result.Clear();
        var array = GetGridsInCircle(worldPosition, radius);
        foreach (var grid in array)
        {
            foreach (var gridObject in grid.GetGridContain<T>())
            {
                result.Add((T)gridObject);
            }
        }
        return result;
    }

    /// <summary>
    /// 对列表进行行为
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void GetCircleGridContainTypeWithAction<T>(Vector3 worldPosition, float radius, System.Action<T> action)
    {
        HashSet<object> result = ObjectPoolManager.Instance.Spawn<HashSet<object>>();
        var array = GetGridsInCircle(worldPosition, radius);
        foreach (var grid in array)
        {
            foreach (var gridObject in grid.GetGridContain<T>())
            {
                result.Add(gridObject);
            }
        }
        foreach(var grid in result)
        {
            action?.Invoke((T)grid);
        }
        result.Clear();
        ObjectPoolManager.Instance.Recycle(result);
    }

    public BaseGrid GetGrid(Vector3 worldPosition)
    {
        var flag = TransferVectorToArrayFlag(worldPosition);
        return searchGrid(flag.Item1, flag.Item2);
    }
    #region GetMethod
    public BaseGrid[] GetGridsInScreenPoints(Vector2[] screenPositions)
    {
        RaycastHit m_HitInfo = new RaycastHit();
        (int, int)[] screenResult = new (int, int)[4];
        HashSet<BaseGrid> set = new HashSet<BaseGrid>();
        Vector2[] checkPoints = new Vector2[4];
        //进行排序

        for (int i = 0; i < screenPositions.Length; i++)
        {
            var ray = Camera.main.ScreenPointToRay(screenPositions[i]);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                if (m_HitInfo.collider == null) return null;
                checkPoints[i] = new Vector2(m_HitInfo.point.x, m_HitInfo.point.z);
                //GameManager.instance.tempShow[i].transform.position = m_HitInfo.point;
                screenResult[i] = TransferVectorToArrayFlag(m_HitInfo.point);
                BaseGrid temp = searchGrid(screenResult[i].Item1, screenResult[i].Item2);
                if (temp != null)
                    set.Add(temp);
            }
        }
        //判断控制点
        //imin+iMax

        int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
        foreach (var point in screenResult)
        {
            if (point.Item1 < minX) minX = point.Item1;
            if (point.Item1 > maxX) maxX = point.Item1;
            if (point.Item2 < minY) minY = point.Item2;
            if (point.Item2 > maxY) maxY = point.Item2;
        }
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                //Debug.LogError($"Add:{i}/{j}");
                BaseGrid temp = searchGrid(i, j);
                if (temp != null)
                    set.Add(temp);
            }
        }
        BaseGrid[] result = new BaseGrid[set.Count];
        set.CopyTo(result);
        return result;
    }

    (int, int) centerPoint;
    (int, int) upPoint;
    (int, int) leftPoint;
    (int, int) rightPoint;
    (int, int) downPoint;
    HashSet<BaseGrid> set = new HashSet<BaseGrid>();
    Vector2 centorV2;
    /// <summary>
    /// radius是真实比例 需要用长度进行缩小
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public HashSet<BaseGrid> GetGridsInCircle(Vector3 worldPosition, float radius)
    {
        centorV2.x = worldPosition.x + center.x;
        centorV2.y = worldPosition.z + center.y;
        //确定中心点
        centerPoint = TransferVectorToArrayFlag(worldPosition);
        //优化
        worldPosition.z += radius;
        upPoint = TransferVectorToArrayFlag(worldPosition);
        worldPosition.z -= radius;
        worldPosition.x -= radius;
        leftPoint = TransferVectorToArrayFlag(worldPosition);
        worldPosition.x += radius;
        worldPosition.x += radius;
        rightPoint = TransferVectorToArrayFlag(worldPosition);
        worldPosition.x -= radius;
        worldPosition.z -= radius;
        downPoint = TransferVectorToArrayFlag(worldPosition);
        worldPosition.z += radius;
        //确定四方点
        BaseGrid temp = searchGrid(upPoint.Item1, upPoint.Item2);
        set.Clear();
        if (temp != null)
            set.Add(temp);
        temp = searchGrid(leftPoint.Item1, leftPoint.Item2);
        if (temp != null)
            set.Add(temp);
        temp = searchGrid(rightPoint.Item1, rightPoint.Item2);
        if (temp != null)
            set.Add(temp);
        temp = searchGrid(downPoint.Item1, downPoint.Item2);
        if (temp != null)
            set.Add(temp);
        temp = searchGrid(centerPoint.Item1, centerPoint.Item2);
        if (temp != null)
            set.Add(temp);
        if (set.Count == 0) return null;
        int yFlag, targetY, leftX, rightX;
        if (!(set.Count == 1 && temp != null))//排除 存在中心点且唯一
        {
            //横向拓展失败后y轴变更
            //拓展搜寻是否有格子 且方位角点在范围内
            yFlag = upPoint.Item2;
            targetY = leftPoint.Item2;
            leftX = upPoint.Item1 - 1;
            rightX = upPoint.Item1 + 1;
            //上到左 上到右 
            while (yFlag != targetY)
            {
                //break;
                while (leftX >= leftPoint.Item1)
                {
                    temp = searchGrid(leftX, yFlag);
                    if (temp == null)//超出边缘
                    {
                        break;
                    }
                    if (temp.GetTargetPositionDistance(GridPosition.SOUTHEAST, centorV2) < radius)
                    {
                        set.Add(temp);
                    }
                    else//右下点不在范围内超出了
                    {
                        break;
                    }
                    leftX--;
                }
                while (rightX <= rightPoint.Item1)
                {
                    temp = searchGrid(rightX, yFlag);
                    if (temp == null)//超出边缘
                    {
                        break;
                    }
                    if (temp.GetTargetPositionDistance(GridPosition.SOUTHWEST, centorV2) < radius)
                    {
                        set.Add(temp);
                    }
                    else//左下点不在范围内超出了
                    {
                        break;
                    }
                    rightX++;
                }
                yFlag--;//Y向下一排移动
                //增加所有leftX到rightY的区间格子
                for (int i = leftX + 1; i < rightX; i++)
                {
                    temp = searchGrid(i, yFlag);
                    if (temp != null)
                        set.Add(temp);
                }
            }
            //下到左 下到右
            yFlag = downPoint.Item2;
            targetY = rightPoint.Item2;
            leftX = upPoint.Item1 - 1;
            rightX = upPoint.Item1 + 1;
            while (yFlag != targetY)
            {
                while (leftX >= leftPoint.Item1)
                {
                    temp = searchGrid(leftX, yFlag);
                    if (temp == null)//超出边缘
                    {
                        break;
                    }
                    if (temp.GetTargetPositionDistance(GridPosition.NORTHEAST, centorV2) < radius)
                    {
                        set.Add(temp);
                    }
                    else//右下点不在范围内超出了
                    {
                        break;
                    }
                    leftX--;
                }
                while (rightX <= rightPoint.Item1)
                {
                    temp = searchGrid(rightX, yFlag);
                    if (temp == null)//超出边缘
                    {
                        break;
                    }
                    if (temp.GetTargetPositionDistance(GridPosition.NORTHWEST, centorV2) < radius)
                    {
                        set.Add(temp);
                    }
                    else//左下点不在范围内超出了
                    {
                        break;
                    }
                    rightX++;
                }
                yFlag++;//Y向上一排移动
                //增加所有leftX到rightY的区间格子
                for (int i = leftX + 1; i < rightX; i++)
                {
                    temp = searchGrid(i, yFlag);
                    if (temp != null)
                        set.Add(temp);
                }
            }
        }
        return set;
    }
    #endregion

    BaseGrid searchGrid(int x, int y)
    {
        if (x < 0 || x >= gridXMax || y < 0 || y > gridYMax)
            return null;
        return map[x, y];
    }

    public (int, int) TransferVectorToArrayFlag(Vector3 worldPosition)
    {
        (int, int) result = (Mathf.FloorToInt((worldPosition.x + center.x) / gridLength), Mathf.FloorToInt((worldPosition.z + center.y) / gridLength));
        return result;
    }
}