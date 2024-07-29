using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础阵型是基于 朝向向下的阵型 
///     |
///     |
///     |    []
///     |[][][][][]
///    y|[][][][][]
///    x ___________________
///         
/// </summary>
public static class FormationUtil
{
    /// <summary>
    /// 中心点偏移的整体原始阵型输出
    /// </summary>
    /// <returns></returns>
    public static Vector3[] GetFormationsWithCenter(int num, float size, float space, int width = -1, FormationType formation = FormationType.SQUARD)
    {

        //TODO:暂时只算方阵
        Vector3[] result = new Vector3[num];

        int hor;
        if (width == -1)
        {//正方形
            hor = (int)Mathf.Sqrt(num);
            if (hor * hor < num) hor++;
        }
        else
        {
            hor = width;
        }
        int line = num / hor + 1;
        //最后一行的余数 如果为0 不用向中靠齐
        int lastLine = num % hor;
        for (int i = 0; i < line - 1; i++)//最后一行手动算，居中
        {
            for (int j = 0; j < hor; j++)
            {
                result[i * hor + j] = new Vector3((size + space) * j, 0, (size + space) * i);
            }
        }

        if (lastLine != 0)
        {
            //中点的x
            float middle = (result[0].x + result[hor - 1].x) / 2;

            for (int j = 0; j < lastLine; j++)
            {
                if (lastLine % 2 == 0)//偶数
                {
                    result[result.Length - lastLine + j] = new Vector3(middle + (j - lastLine / 2 + 0.5f) * (size + space), 0, (line - 1) * (size + space));
                }
                else
                {
                    result[result.Length - lastLine + j] = new Vector3(middle + (j - lastLine / 2) * (size + space), 0, (line - 1) * (size + space));
                }
            }

        }

        //做 第一行中间的偏差值
        Vector3 centerDelta = Vector3.zero;
        centerDelta.x = (result[0].x + result[hor - 1].x) / 2;
        centerDelta.z = (result[0].z + result[result.Length - 1].z) / 2;

        //运算中心点偏移
        for (int i = 0; i < result.Length; i++)
        {
            result[i] -= centerDelta;
        }
        return result;

    }

    /// <summary>
    /// 计算第一行中心点的偏移队形
    /// </summary>
    /// <param name="num">总体数量</param>
    /// <param name="size"></param>
    /// <param name="space"></param>
    /// <param name="width"></param>
    /// <param name="formation"></param>
    /// <returns></returns>
    public static Vector3[] GetFormationsWithFirstLineCenter(int num, float size, float space, int width = -1, FormationType formation = FormationType.SQUARD)
    {
        //TODO:暂时只算方阵
        Vector3[] result = new Vector3[num];
        int hor;
        if (width == -1)
        {//正方形
            hor = (int)Mathf.Sqrt(num);
            if (hor * hor < num) hor++;
        }
        else
        {
            hor = width;
        }
        int line = num / hor + 1;
        //最后一行的余数 如果为0 不用向中靠齐
        int lastLine = num % hor;
        for (int i = 0; i < line - 1; i++)//最后一行手动算，居中
        {
            for (int j = 0; j < hor; j++)
            {
                result[i * hor + j] = new Vector3((size + space) * j, 0, (size + space) * i);
            }
        }

        if (lastLine != 0)
        {
            //中点的x
            float middle = (result[0].x + result[hor - 1].x) / 2;

            for (int j = 0; j < lastLine; j++)
            {
                if (lastLine % 2 == 0)//偶数
                {
                    result[result.Length - lastLine + j] = new Vector3(middle + (j - lastLine / 2 + 0.5f) * (size + space), 0, (line - 1) * (size + space));
                }
                else
                {
                    result[result.Length - lastLine + j] = new Vector3(middle + (j - lastLine / 2) * (size + space), 0, (line - 1) * (size + space));
                }
            }

        }

        //做 第一行中间的偏差值
        Vector3 centerDelta = Vector3.zero;
        centerDelta.x = (result[0].x + result[hor - 1].x) / 2;
        centerDelta.z = (result[0].z);

        //运算中心点偏移
        for (int i = 0; i < result.Length; i++)
        {
            result[i] -= centerDelta;
        }
        return result;

    }

    /// <summary>
    /// 计算第一行中心点的偏移队形
    /// </summary>
    /// <param name="num"></param>
    /// <param name="space"></param>
    /// <returns></returns>
    public static Vector3[] GetFormationsWithFirstLineCenter(int num, float[] space)
    {
        //TODO:暂时只算方阵
        Vector3[] result = new Vector3[num];

        Vector3 t = new Vector3(0, 0, 0);
        result[0] = t;
        if (space == null)
            return result;

        //最后一行的余数 如果为0 不用向中靠齐

        for (int i = 0; i < space.Length; i++)//最后一行手动算，居中
        {

            t += new Vector3(space[i], 0, 0);
            result[i + 1] = t;
        }

        //做 第一行中间的偏差值
        Vector3 centerDelta = Vector3.zero;
        centerDelta.x = (result[0].x + result[num - 1].x) / 2;


        //运算中心点偏移
        for (int i = 0; i < result.Length; i++)
        {
            result[i] -= centerDelta;
        }
        return result;

    }

    /// <summary>
    /// 计算右下角的偏移队形
    /// </summary>
    /// <param name="num">总体数量</param>
    /// <param name="size"></param>
    /// <param name="space"></param>
    /// <param name="width"></param>
    /// <param name="formation"></param>
    /// <returns></returns>
    public static Vector3[] GetFormationsWithFirstLine(int num, float size, float space, int width = -1, FormationType formation = FormationType.SQUARD)
    {
        //TODO:暂时只算方阵
        Vector3[] result = new Vector3[num];
        int hor;
        if (width == -1)
        {//正方形
            hor = (int)Mathf.Sqrt(num);
            if (hor * hor < num) hor++;
        }
        else
        {
            hor = width;
        }
        int line = num / hor + 1;
        //最后一行的余数 如果为0 不用向中靠齐
        int lastLine = num % hor;
        for (int i = 0; i < line - 1; i++)//最后一行手动算，居中
        {
            for (int j = 0; j < hor; j++)
            {
                result[i * hor + j] = new Vector3((size + space) * j, 0, (size + space) * i);
            }
        }

        if (lastLine != 0)
        {
            //中点的x
            float middle = (result[0].x + result[hor - 1].x) / 2;

            for (int j = 0; j < lastLine; j++)
            {
                if (lastLine % 2 == 0)//偶数
                {
                    result[result.Length - lastLine + j] = new Vector3(middle + (j - lastLine / 2 + 0.5f) * (size + space), 0, (line - 1) * (size + space));
                }
                else
                {
                    result[result.Length - lastLine + j] = new Vector3(middle + (j - lastLine / 2) * (size + space), 0, (line - 1) * (size + space));
                }
            }

        }

        //做 第一行最后一个的偏差值
        Vector3 theLastOneInTheFirstRow = Vector3.zero;
        theLastOneInTheFirstRow.x = (result[hor - 1].x);


        //运算中心点偏移
        for (int i = 0; i < result.Length; i++)
        {
            result[i] -= theLastOneInTheFirstRow;
        }
        return result;

    }

    public static Vector2[] BoderPointShrink(Vector2[] borderPoints, Vector2 centorPoint, float shrinkDistance)
    {
        for (int i = 0; i < borderPoints.Length; i++)
        {
            borderPoints[i] = borderPoints[i] + (centorPoint - borderPoints[i]).normalized * shrinkDistance;
        }
        return borderPoints;
    }
    /// <summary>
    /// 计算右下角的偏移队形
    /// </summary>
    /// <param name="num"></param>
    /// <param name="space"></param>
    /// <returns></returns>
    public static Vector3[] GetFormationsWithFirstLine(int num, float[] space)
    {
        //TODO:暂时只算方阵
        Vector3[] result = new Vector3[num];

        result[0] = new Vector3(0, 0, 0);

        if (space == null)
            return result;

        //最后一行的余数 如果为0 不用向中靠齐

        for (int i = 1; i < num; i++)//最后一行手动算，居中
        {

            result[i] = result[i - 1] + new Vector3(space[i - 1], 0, 0);

        }

        //做 第一行中间的偏差值

        return result;

    }

    /// <summary>
    /// 计算基于阵型右下角的偏移队形
    /// </summary>
    /// <param name="commands"></param>
    /// <param name="totalWidth"></param>
    /// <param name="resultFlagArray"></param>
    /// <returns></returns>
    public static Vector3[] GetFormationsWithLowerRight(List<CommandUnit> commands, float totalWidth, int[] resultFlagArray, ref Dictionary<CommandUnit, int> commandWidthDic)
    {



        #region 当前宽度 小于最小宽度时
        float minWidth = 0;

        for (int i = 0; i < commands.Count; i++)
        {
            minWidth += commands[i].MinWidth * (commands[i].moduleSize + commands[i].spaceSize);
        }
        minWidth += (commands.Count - 1) * 1.5f;


        if (minWidth >= totalWidth)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                commandWidthDic[commands[i]] = commands[i].MinWidth;
            }

            return GetFormationsWithFirstLine(commands.Count, GetSizeAndSpaceWithLowerRight(commands, resultFlagArray, commandWidthDic));

        }
        #endregion

        //先计算最大距离 再与当前距离做比较



        float maxWidth = (commands.Count - 1) * 1.5f;

        for (int i = 0; i < commands.Count; i++)
        {
            maxWidth += commands[i].MaxWidth * (commands[i].moduleSize + commands[i].spaceSize);
        }

        if (totalWidth < maxWidth)
        {
            #region 当前距离大于最小距离小于最大距离时

            //计算当前队伍宽度
            float nowWidth = minWidth;
            for (int i = 0; i < commands.Count; i++)
            {
                commandWidthDic[commands[i]] = commands[i].MinWidth;
            }
            int index = 0;
            //当前宽度与实际宽度作比较
            while (nowWidth < totalWidth)
            {
                if (!(commandWidthDic[commands[resultFlagArray[index]]] == commands[resultFlagArray[index]].MaxWidth))
                {
                    nowWidth += commands[resultFlagArray[index]].moduleSize + commands[resultFlagArray[index]].spaceSize;
                    commandWidthDic[commands[resultFlagArray[index]]]++;
                }
                index++;
                if (index == commands.Count)
                    index = 0;
            }
            return GetFormationsWithFirstLine(commands.Count, GetSizeAndSpaceWithLowerRight(commands, resultFlagArray, commandWidthDic));

            #endregion
        }
        else
        {
            #region 当前距离大于最大距离时
            for (int i = 0; i < commands.Count; i++)
            {
                commandWidthDic[commands[i]] = commands[i].MaxWidth;
            }
            return GetFormationsWithFirstLine(commands.Count, GetSizeAndSpaceWithLowerRight(commands, resultFlagArray, commandWidthDic));
            #endregion
        }

    }

    /// <summary>
    /// 计算部队中心位置在大队中的位置偏移 
    /// </summary>
    /// <param name="tempCommand"></param>
    /// <param name="resultFlagArray"></param>
    /// <returns></returns>
    public static float[] GetSizeAndSpaceWithLineCenter(List<CommandUnit> tempCommand, int[] resultFlagArray)
    {
        if (tempCommand.Count == 1)
            return null;

        float[] result = new float[tempCommand.Count - 1];
        //n-1是因为 第一队列与第二队列才产生1个space
        for (int i = 0; i < tempCommand.Count - 1; i++)
        {
            result[i] = (tempCommand[resultFlagArray[i]].overallWidth + tempCommand[resultFlagArray[i + 1]].overallWidth) / 2 + 2f;
        }
        return result;
    }

    /// <summary>
    /// 计算基于阵型右下角的偏移队形
    /// </summary>
    /// <param name="tempCommand"></param>
    /// <param name="resultFlagArray"></param>
    /// <returns></returns>
    public static float[] GetSizeAndSpaceWithLowerRight(List<CommandUnit> tempCommand, int[] resultFlagArray, Dictionary<CommandUnit, int> commandWidthDic)
    {
        if (tempCommand.Count == 1)
            return null;

        float[] result = new float[tempCommand.Count - 1];
        //n-1是因为 第一队列与第二队列才产生1个space
        for (int i = 0; i < tempCommand.Count - 1; i++)
        {
            result[i] = commandWidthDic[tempCommand[resultFlagArray[i]]] * (tempCommand[resultFlagArray[i]].spaceSize + tempCommand[resultFlagArray[i]].moduleSize) - tempCommand[resultFlagArray[i]].spaceSize + 2f;
        }


        return result;
    }

    public static Vector3[] GetLeftFormations(int num, float size, float space, int width = -1, FormationType formation = FormationType.SQUARD)
    {
        //TODO:暂时只算方阵
        Vector3[] result = new Vector3[num];
        int hor;
        if (width == -1)
        {//正方形
            hor = (int)Mathf.Sqrt(num);
            if (hor * hor < num) hor++;
        }
        else
        {
            hor = width;
        }
        int left = num % hor;
        int line = num / hor + 1;
        for (int i = 0; i < line - 1; i++)//最后一行手动算，居中
        {
            for (int j = 0; j < hor; j++)
            {
                result[i * hor + j] = new Vector3((size + space) * j, 0, (size + space) * i);
            }
        }
        Vector3 centerDelta = Vector3.zero;
        if (left != 0)
        {
            //float halfLine = line / 2;
            float middle = (result[0].x + result[hor - 1].x) / 2;
            for (int j = 0; j < left; j++)
            {
                int delta = left / 2;
                if (left % 2 == 0)//偶数
                {
                    result[result.Length - left + j] = new Vector3(middle + (j - left / 2 + 0.5f) * (size + space), 0, (line - 1) * (size + space));
                }
                else
                {
                    result[result.Length - left + j] = new Vector3(middle + (j - left / 2) * (size + space), 0, (line - 1) * (size + space));
                }
                //尝试进行居中
            }
        }
        centerDelta.x = (result[0].x);
        centerDelta.z = (result[0].z);
        Debug.Log(centerDelta);
        //运算中心点偏移
        for (int i = 0; i < result.Length; i++)
        {
            result[i] -= centerDelta;
        }
        return result;
    }

    public static SquardFormation GetSquardFormation(int num, int width = -1)
    {
        SquardFormation result = new SquardFormation();
        if (width == -1)
        {//正方形
            result.row = (int)Mathf.Sqrt(num);
            if (result.row * result.row < num)
                result.row++;
            else
            {
                result.line = result.row;
                return result;
            }
        }
        else
        {
            result.row = width;
        }
        result.left = num % result.row;
        result.line = num / result.row;
        //if (result.left > 0) result.left++;
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="leader"></param>
    /// <param name="formation"></param>
    /// <param name="faceTo"></param>
    /// <param name="leaderFlagLock">领队是否进行强锁</param>
    public static int[] ResetPosition(CommandUnit leader, SquardFormation formation, Vector3 faceTo)
    {

        int[] list = leader.GetAllTroopFlagList();
        Vector3[] positions = leader.GetAllTroopPositionList();
        //获得投影的中轴
        float[] lineLength = ArrayUtil.GetVectorProjection(positions, faceTo, 0, positions.Length);//求得所有单位的位置
        //越大= 越合适，应该进行逆序
        ArrayUtil.ListSortByOuter(list, lineLength, 0);
        //Y轴旋转-90度，从大到小 按行排列
        faceTo = Quaternion.Euler(0, 90, 0) * faceTo;//TODO：增加左向与右向的各自不同阵型评级
        Vector3[] shiftPositions = new Vector3[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            shiftPositions[i] = positions[list[i]];
        }
        positions = shiftPositions;
        int countDownLine = formation.line;
        for (int i = 0; i < countDownLine; i++)
        {
            //分段分组
            int startIndex = i * formation.row;
            int endIndex = startIndex + formation.row;
            lineLength = ArrayUtil.GetVectorProjection(positions, faceTo, startIndex, endIndex);//部分垂直投影数组
            ArrayUtil.ListSortByOuter(list, lineLength, startIndex);//分批排序
        }
        if (formation.left != 0)
        {
            int startIndex = formation.row * formation.line;
            int endIndex = startIndex + formation.left;
            lineLength = ArrayUtil.GetVectorProjection(positions, faceTo, startIndex, endIndex);//部分垂直投影数组
            ArrayUtil.ListSortByOuter(list, lineLength, startIndex);//分批排序
        }
        //然后根据行再排列
        //最后返还一个排序完毕的对照int列表
        return list;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="leader"></param>
    /// <param name="formation"></param>
    /// <param name="faceTo"></param>
    /// <param name="leaderFlagLock">领队是否进行强锁</param>
    public static int[] FitTargetPositions(Vector3[] positions, SquardFormation formation, Vector3 faceTo)
    {
        int[] list = new int[positions.Length];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = i;
        }
        //获得投影的中轴
        float[] lineLength = ArrayUtil.GetVectorProjection(positions, faceTo, 0, positions.Length);//求得所有单位的位置
        //越大= 越合适，应该进行逆序
        ArrayUtil.ListSortByOuter(list, lineLength, 0);
        //Y轴旋转-90度，从大到小 按行排列
        faceTo = Quaternion.Euler(0, 90, 0) * faceTo;//TODO：增加左向与右向的各自不同阵型评级
        Vector3[] shiftPositions = new Vector3[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            shiftPositions[i] = positions[list[i]];
        }
        positions = shiftPositions;
        int countDownLine = formation.line;
        for (int i = 0; i < countDownLine; i++)
        {
            //分段分组
            int startIndex = i * formation.row;
            int endIndex = startIndex + formation.row;
            lineLength = ArrayUtil.GetVectorProjection(positions, faceTo, startIndex, endIndex);//部分垂直投影数组
            ArrayUtil.ListSortByOuter(list, lineLength, startIndex);//分批排序
        }
        if (formation.left != 0)
        {
            int startIndex = formation.row * formation.line;
            int endIndex = startIndex + formation.left;
            lineLength = ArrayUtil.GetVectorProjection(positions, faceTo, startIndex, endIndex);//部分垂直投影数组
            ArrayUtil.ListSortByOuter(list, lineLength, startIndex);//分批排序
        }
        //然后根据行再排列
        //最后返还一个排序完毕的对照int列表
        return list;
    }


    public static int[] FitTargetPositions(List<CommandUnit> commands, SquardFormation formation, Vector3 faceTo)
    {

        var positions = new Vector3[commands.Count];
        for (int i = 0; i < commands.Count; i++)
        {
            positions[i] = commands[i].lastPosition;

        }

        return FitTargetPositions(positions, formation, faceTo);
    }

    public static Vector3[] FormationChangeFace(Vector3[] originData, Vector3 targetVector, float angleFix = 0f)
    {
        float angle = Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg + 180;
        Quaternion rotateMethod = Quaternion.Euler(0, angle + angleFix, 0);
        for (int i = 0; i < originData.Length; i++)
        {
            originData[i] = rotateMethod * originData[i];
        }
        return originData;
    }
}
public class SquardFormation
{
    public int row;
    public int line;
    public int left;

    public float[] GetVectorProjection(Vector3[] targetPositions, Vector3 faceTo)
    {
        float[] lines = new float[line];
        Vector3 targetPosition;
        faceTo.y = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            targetPosition = targetPositions[i * row];
            targetPosition.y = 0;
            lines[i] = faceTo.ProjectLength(targetPosition);
        }
        return lines;
    }
    /// <summary>
    /// 开区间
    /// </summary>
    public float[] GetVectorProjection(Vector3[] targetPositions, Vector3 faceTo, int start, int end)
    {
        float[] lines = new float[end - start - 1];
        Vector3 targetPosition;
        faceTo.y = 0;
        for (int i = start; i < end; i++)
        {
            targetPosition = targetPositions[i * row];
            targetPosition.y = 0;
            lines[i] = faceTo.ProjectLength(targetPosition);
        }
        return lines;
    }
}
public enum FormationType
{
    SQUARD,//方阵
    VELITES,//散阵
}
