using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectorMapCreator : MonoSingleton<SectorMapCreator>
{
    public Texture2D sourceImage;
    //转换成边界点的图源
    public Transform parentGameObjectPrefab;
    /// <summary>
    /// 边界值
    /// </summary>
    public List<Border> borders = new List<Border>();
    /// <summary>
    /// 待处理List
    /// </summary>
    public List<Border> openList = new List<Border>();
    /// <summary>
    /// 是否已处理
    /// </summary>
    bool[,] hasProcess;
    public Transform borderParent;
    public Material lineMaterial;
    HashSet<Color> colorSet = new HashSet<Color>();
    HashSet<Color> totalColorSet = new HashSet<Color>();

    /// <summary>
    /// 当前搜索的边界
    /// </summary>
    Border nowCheckborder;

    //每两个颜色构成一种边界
    public GameObject sectorPrefab;

    /// <summary>
    /// 根据颜色索引的字典
    /// </summary>
    Dictionary<Color, SectorBlock> blockDic = new Dictionary<Color, SectorBlock>();
    Dictionary<Color, SectorUI> uiDic = new Dictionary<Color, SectorUI>();

    public void OnCreateBounds()
    {
        OnCreateBounds(null);
    }
    /// <summary>
    /// 创建颜色便捷
    /// </summary>
    public void OnCreateBounds(Texture2D targetImage = null)
    {
        if (targetImage != null) sourceImage = targetImage;
        int width = sourceImage.width;
        int height = sourceImage.height;
        hasProcess = new bool[width + 1, height + 1];
        for (int j = 0; j <= height; j++)
        {
            for (int i = 0; i <= width; i++)
            {
                ProcessPoint(i, j);
            }
        }
        hasProcess = new bool[width + 1, height + 1];
        ProcessBorder();

        //根据边框进行图像的输出
        List<Color> colorList = new List<Color>(totalColorSet);
        foreach (var color in colorList)
        {
            if (color.Equals(Color.black)) continue;
            SectorBlock sectorblock = new SectorBlock(color);
            blockDic.Add(color, sectorblock);
        }
        int count = 0;
        foreach (var border in borders)
        {
            count++;
            var borderObject = new GameObject();
            borderObject.transform.parent = borderParent;
            borderObject.name = $"Border{count}";
            borderObject.transform.localPosition = Vector3.zero;
            borderObject.transform.localEulerAngles = Vector3.zero;
            if (border.isBlackBorder) borderObject.gameObject.SetActive(false);
            var line = borderObject.AddComponent<LineRenderer>();
            border.borderObject = borderObject;
            Vector3[] points = new Vector3[border.pointsNum];
            int flag = 0;
            border.PointsAction((r) =>
            {
                points[flag] = new Vector3(r.Item1, r.Item2, 0);
                flag++;
            });
            line.sortingOrder = 1;
            line.positionCount = points.Length;
            line.SetPositions(points);
            line.useWorldSpace = false;
            line.Simplify(0.8f);
            line.widthMultiplier = 0.2f;
            line.numCornerVertices = 5;
            line.material = lineMaterial;
            Vector3[] result = new Vector3[line.positionCount];
            line.GetPositions(result);

            var temp = UsePath2DToProcessPoints(result);
            line.positionCount = temp.Length;
            line.SetPositions(temp);
            result = new Vector3[line.positionCount];
            line.GetPositions(result);

            border.SetPoints(result);
            if (blockDic.TryGetValue(border.mainColor, out SectorBlock block))
            {
                block.InsertUIPoints(result.Depress());
                block.borders.Add(border);
            }
            if (border.mainColor != border.otherColor)
            {
                if (blockDic.TryGetValue(border.otherColor, out block))
                {
                    block.InsertUIPoints(result.Depress());
                    block.borders.Add(border);
                }

            }
        }
        bool hasInsertBlock = false;
        foreach (var bp in blockDic)
        {
            SectorBlock block = bp.Value;
            block.CheckOverlapPoint();
            block.PointsSequence();

            var g = Instantiate(sectorPrefab.gameObject, parentGameObjectPrefab);
            (g.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            g.name = "Sector" + bp.Value.recognizeColor.ToString();
            var eg = g.GetComponent<EditableGraphic>();
            Vector2[] points = bp.Value.ExtracBorderPoints();
            var tris = eg.InsertPointsData(points);//获得切分后三角形分布
            block.tris = MathUtil.Delaun(points);
            DelaunayTriangle.instance.InsertNewPoint(points);
            //if (!hasInsertBlock)
            //{
            //    hasInsertBlock = true;
            //    DelaunayTriangle.instance.InsertNewPoint(points);
            //}
            //构筑内部 耳切后的Triangle寻路网格
            var ui = g.AddComponent<SectorUI>();
            //ui.InitTargetColor(ColorUtil.sectorDefaultColor);//需要地图数据
            ui.ui = eg;
            ui.recognizeColor = block.recognizeColor;
            uiDic.Add(ui.recognizeColor, ui);
            block.gravityPoint = GraphicUtil.GetGravityPoint(points);
            //将相关的边进行存储
        }
#if UNITY_EDITOR
        foreach (var bp in blockDic)
        {
            SectorBlock block = bp.Value;

            break;
            //if (useSegmentColor)
            //{
            //    foreach (var tri in block.meshTris)
            //    {
            //        var gt = Instantiate(sectorPrefab.gameObject, parentGameObjectPrefab);
            //        (gt.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            //        gt.name = "tri";
            //        var eg = gt.GetComponent<EditableGraphic>();
            //        eg.InsertPointsData(tri.points);
            //        eg.color = Color.white;
            //        tri.graphic = eg;
            //    }
            //}
        }
#endif
        SectorBlockManager.Instance.RegisterNowMapData(blockDic, sourceImage);
        SectorBlockManager.Instance.RegisterNowUIData(uiDic);
    }

    Vector3[] UsePath2DToProcessPoints(Vector3[] points)
    {
        Vector2[] result = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            result[i] = points[i];
        }
        Path2D path = new Path2D(result);
        path.AutoSetControlPoints = true;
        result = path.CalculateEvenlySapcedPoints(1f, 1);
        points = new Vector3[result.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = result[i];
        }
        return points;
    }

    /// <summary>
    /// 边界修正
    /// </summary>
    void ProcessBorder()
    {
        List<Color> nowBorderColors = new List<Color>();
        List<Border> borders = new List<Border>();
        //从零零点开始
        SetPointColorSet(0, 0);
        Color pureColor = ArrayUtil.GetHashSetFirst(colorSet);
        var b = borders.Find((target) => target.MatchTargetColor(pureColor));
        Border newBorder = new Border(pureColor, pureColor);
        newBorder.InsertPoint(0, 0);
        openList.Add(newBorder);
        ExpendBorderList();
    }

    void ExpendBorderList()
    {
        nowCheckborder = openList[0];
        //————
        //从openList里面移除转入正式border内部
        nowCheckborder.id = borders.Count + 1;
        borders.Add(nowCheckborder);
        openList.Remove(nowCheckborder);
        (int, int) startPoint = nowCheckborder.GetFirstPoint();
        hasProcess[startPoint.Item1, startPoint.Item2] = true;//第一个点标注
        (int, int) nextComparePoint = FindNextBorderPoint(startPoint, true);
        //判断
        while (nextComparePoint != default)
        {
            hasProcess[nextComparePoint.Item1, nextComparePoint.Item2] = true;
            nowCheckborder.InsertPoint(nextComparePoint.Item1, nextComparePoint.Item2);
            nextComparePoint = FindNextBorderPoint(nextComparePoint, true);
        }
        (int, int) preComparePoint = FindNextBorderPoint(startPoint, false);
        while (preComparePoint != default)
        {
            hasProcess[preComparePoint.Item1, preComparePoint.Item2] = true;
            nowCheckborder.InsertLastPoint(preComparePoint.Item1, preComparePoint.Item2);
            preComparePoint = FindNextBorderPoint(preComparePoint, false);
        }
        if (openList.Count > 0)
        {
            ExpendBorderList();
        }
    }

    /// <summary>
    /// 处理点的运算
    /// </summary>
    void ProcessPoint(int width, int height)
    {
        if (hasProcess[width, height]) return;
        //是否是已经处理过的点——是、Pass
        SetPointColorSet(width, height);
        if (colorSet.Count <= 1) return;//内部颜色点
        foreach (var colorA in colorSet)
        {
            foreach (var colorB in colorSet)
            {
                if (colorA.CompareTo(colorB) != 1) continue;//去重：小于与等于
                //Debug.LogError($"AddNewOpen:{colorA} {colorB}");
                Border newBorder = new Border(colorA, colorB);
                newBorder.InsertPoint(width, height);
                openList.Add(newBorder);
            }
        }
        if (openList.Count > 0)
        {
            ExpendOpenList();
        }
    }

    /// <summary>
    /// 根据点进行颜色数据的获取
    /// </summary>
    void SetPointColorSet(int width, int height)
    {
        colorSet.Clear();
        if (width > 0 && height > 0)
            colorSet.Add(sourceImage.GetPixel(width - 1, height - 1));
        if (width > 0 && height != sourceImage.height)
            colorSet.Add(sourceImage.GetPixel(width - 1, height));
        if (width < sourceImage.width && height > 0)
            colorSet.Add(sourceImage.GetPixel(width, height - 1));
        if (width < sourceImage.width && height < sourceImage.height)
            colorSet.Add(sourceImage.GetPixel(width, height));
        foreach (var color in colorSet)
        {
            totalColorSet.Add(color);
        }
    }

    /// <summary>
    /// 拓展开放列表内的内容
    /// </summary>
    void ExpendOpenList()
    {
        nowCheckborder = openList[0];
        //————
        //从openList里面移除转入正式border内部
        nowCheckborder.id = borders.Count + 1;
        borders.Add(nowCheckborder);
        openList.Remove(nowCheckborder);
        (int, int) startPoint = nowCheckborder.GetFirstPoint();
        hasProcess[startPoint.Item1, startPoint.Item2] = true;//第一个点标注
        //+x +y扫描线形式向上
        //右 上 左 下 形式衍生
        lastFace = -1;//数据重置
        (int, int) nextComparePoint = FindNextComparePoint(startPoint);
        //判断
        while (nextComparePoint != default)
        {
            hasProcess[nextComparePoint.Item1, nextComparePoint.Item2] = true;
            nowCheckborder.InsertPoint(nextComparePoint.Item1, nextComparePoint.Item2);
            nextComparePoint = FindNextComparePoint(nextComparePoint);
        }
        lastFace = -1;
        (int, int) preComparePoint = FindNextComparePoint(startPoint);
        while (preComparePoint != default)
        {
            hasProcess[preComparePoint.Item1, preComparePoint.Item2] = true;
            nowCheckborder.InsertLastPoint(preComparePoint.Item1, preComparePoint.Item2);
            preComparePoint = FindNextComparePoint(preComparePoint);
        }
        if (openList.Count > 0)
        {
            ExpendOpenList();
        }
    }

    int lastFace = -1;
    /// <summary>
    /// 获取下一个关联点
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    (int, int) FindNextComparePoint((int, int) point)
    {
        for (int i = 0; i < 4; i++)
        {
            int flag = i;
            if (lastFace == 0 || lastFace == 1)//左右情况优先判定上下
            {
                flag += 2;
                flag %= 4;
            }
            switch (flag)
            {
                //右
                case 0:
                    if (lastFace != 1 && IsNeedPaint(point.Item1 + 1, point.Item2))
                    {
                        //判断两点之间的左右颜色是否连续（避免跳跃点）
                        //起始点的右上与右下不得同色
                        if (sourceImage.GetPixel(point.Item1, point.Item2).Equals(sourceImage.GetPixel(point.Item1, point.Item2 - 1)))
                        {
                            continue;
                        }
                        lastFace = 0;//下次优先下
                        return (point.Item1 + 1, point.Item2);
                    }
                    break;
                //左
                case 1:
                    if (lastFace != 0 && IsNeedPaint(point.Item1 - 1, point.Item2))
                    {
                        if (sourceImage.GetPixel(point.Item1 - 1, point.Item2).Equals(sourceImage.GetPixel(point.Item1 - 1, point.Item2 - 1)))//左上左下
                        {
                            continue;
                        }
                        lastFace = 1;//下次优先上
                        return (point.Item1 - 1, point.Item2);
                    }
                    break;
                //上
                case 2:
                    if (lastFace != 3 && IsNeedPaint(point.Item1, point.Item2 + 1))
                    {
                        if (sourceImage.GetPixel(point.Item1 - 1, point.Item2).Equals(sourceImage.GetPixel(point.Item1, point.Item2)))//左上右上
                        {
                            continue;
                        }
                        lastFace = 2;//下次优先右
                        return (point.Item1, point.Item2 + 1);
                    }
                    break;
                //下
                case 3:
                    if (lastFace != 2 && IsNeedPaint(point.Item1, point.Item2 - 1))
                    {
                        if (sourceImage.GetPixel(point.Item1 - 1, point.Item2 - 1).Equals(sourceImage.GetPixel(point.Item1, point.Item2 - 1)))//左下右下
                        {
                            continue;
                        }
                        lastFace = 3;//下次优先左
                        return (point.Item1, point.Item2 - 1);
                    }
                    break;
            }
        }
        return default;
    }

    (int, int) FindNextBorderPoint((int, int) point, bool isClockwise = false)
    {
        if (isClockwise)
        {
            //四种情况
            if (point.Item1 == 0)
            {
                if (point.Item2 < sourceImage.height)
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 + 1))
                        return (point.Item1, point.Item2 + 1);//顺时针向上
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1 + 1, point.Item2))
                        return (point.Item1 + 1, point.Item2);
                }
            }
            else if (point.Item1 == sourceImage.width)
            {
                if (point.Item2 > 0)
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 - 1))
                        return (point.Item1, point.Item2 - 1);//顺时针向下
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1 - 1, point.Item2))
                        return (point.Item1 - 1, point.Item2);
                }
            }
            else if (point.Item2 == 0)
            {
                if (point.Item1 > 0)
                {
                    if (IsNeedBorderPoint(point.Item1 - 1, point.Item2))
                        return (point.Item1 - 1, point.Item2);//顺时针向左
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 + 1))
                        return (point.Item1, point.Item2 + 1);
                }
            }
            else if (point.Item2 == sourceImage.height)
            {
                if (point.Item1 < sourceImage.width)
                {
                    if (IsNeedBorderPoint(point.Item1 + 1, point.Item2))
                        return (point.Item1 + 1, point.Item2);//顺时针向右
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 - 1))
                        return (point.Item1, point.Item2 - 1);
                }
            }
        }
        else
        {
            //四种情况
            if (point.Item1 == 0)
            {
                if (point.Item2 > 0)
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 - 1))
                        return (point.Item1, point.Item2 - 1);//逆时针向下
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1 + 1, point.Item2))
                        return (point.Item1 + 1, point.Item2);
                }
            }
            else if (point.Item1 == sourceImage.width)
            {
                if (point.Item2 < sourceImage.height)
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 + 1))
                        return (point.Item1, point.Item2 + 1);//逆时针向上
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1 - 1, point.Item2))
                        return (point.Item1 - 1, point.Item2);
                }
            }
            else if (point.Item2 == 0)
            {
                if (point.Item1 < sourceImage.width)
                {
                    if (IsNeedBorderPoint(point.Item1 + 1, point.Item2))
                        return (point.Item1 + 1, point.Item2);//逆时针向右
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 + 1))
                        return (point.Item1, point.Item2 + 1);
                }
            }
            else if (point.Item2 == sourceImage.height)
            {
                if (point.Item1 > 0)
                {
                    if (IsNeedBorderPoint(point.Item1 - 1, point.Item2))
                        return (point.Item1 - 1, point.Item2);//逆时针向左
                }
                else
                {
                    if (IsNeedBorderPoint(point.Item1, point.Item2 - 1))
                        return (point.Item1, point.Item2 - 1);
                }
            }
        }
        return default;
    }

    bool IsNeedBorderPoint(int width, int height)
    {
        try
        {
            if (hasProcess[width, height])
            {
                //寻找相同位置的单个端点合并
                foreach (var border in openList)
                {
                    if (border.pointsNum == 1 && border.MatchTargetColor(nowCheckborder))
                    {
                        var point = border.GetFirstPoint();
                        if (point.Item1 == width && point.Item2 == height)
                        {
                            openList.Remove(border);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        catch
        {
            Debug.LogError($"{width} {height}");
        }
        SetPointColorSet(width, height);
        if (hasProcess[width, height] && colorSet.Count == 1)
        {
            //兼容双色点 单色点不兼容
            return false;
        }
        bool result = false;
        foreach (var c in colorSet)
        {
            if (nowCheckborder.MatchTargetColor(c))
            {
                result = true;
                break;
            }
        }
        if (colorSet.Count > 1)
        {
            foreach (var c in colorSet)
            {
                if (!nowCheckborder.MatchTargetColor(c) && !hasProcess[width, height])//未注册过的点
                {
                    Border newBorder = new Border(c, c);
                    newBorder.InsertPoint(width, height);
                    openList.Add(newBorder);
                }
            }
        }
        return result;
    }

    bool IsNeedPaint(int width, int height)
    {
        if (width < 0 || height < 0 || width > sourceImage.width || height > sourceImage.height) return false;
        try
        {
            //51 42 0
            if (hasProcess[width, height])
            {
                //寻找相同位置的单个端点合并
                foreach (var border in openList)
                {
                    if (border.pointsNum == 1 && border.MatchTargetColor(nowCheckborder))
                    {
                        var point = border.GetFirstPoint();
                        if (point.Item1 == width && point.Item2 == height)
                        {
                            openList.Remove(border);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        catch
        {
            Debug.LogError($"{width} {height}");
        }
        //边界点
        SetPointColorSet(width, height);
        if (colorSet.Count <= 1) return false;

        int matchTimes = 0;
        foreach (var color in colorSet)
        {
            if (nowCheckborder.MatchTargetColor(color))
            {
                matchTimes++;
            }
        }
        if (matchTimes == 2)//符合两种颜色
        {
            if (colorSet.Count >= 3)//视作端点，尝试移除占位的重复
            {
                //拓展OpenList
                foreach (var colorA in colorSet)
                {
                    foreach (var colorB in colorSet)
                    {
                        if (colorA.CompareTo(colorB) != 1) continue;//
                        Border newBorder = new Border(colorA, colorB);
                        //Debug.LogError($"AddNewOpen:{colorA} {colorB}");
                        newBorder.InsertPoint(width, height);
                        openList.Add(newBorder);
                    }
                }
                RemoveMatchPointBorder(width, height);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    void RemoveMatchPointBorder(int width, int height)
    {
        foreach (var border in openList)
        {
            if (border.MatchTargetColor(nowCheckborder))
            {
                var point = border.GetFirstPoint();
                if (point.Item1 == width && point.Item2 == height)
                {
                    openList.Remove(border);
                    break;
                }
            }
        }
    }

    public Border GetTargetBorder(Color firstColor, Color secondColor)
    {
        for (int i = 0; i < borders.Count; i++)
        {
            if (borders[i].MatchTargetColor(firstColor, secondColor))
            {
                return borders[i];
            }
        }
        //创建新的border+列入序列表
        Border newBorder = new Border(firstColor, secondColor);
        openList.Add(newBorder);
        borders.Add(newBorder);
        return newBorder;
    }
}
