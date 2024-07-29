using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayUtil
{
    public static T GetRandomElement<T>(this T[] array)
    {
        int index = UnityEngine.Random.Range(0, array.Length);
        return array[index];
    }
    /// <summary>
    /// 打乱重排
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int flag = UnityEngine.Random.Range(i, array.Length);
            T temp = array[flag];
            array[flag] = array[i];
            array[i] = temp;
        }
    }
    public static void Shuffle<T>(this List<T> array)
    {
        for (int i = 0; i < array.Count; i++)
        {
            int flag = UnityEngine.Random.Range(i, array.Count);
            T temp = array[flag];
            array[flag] = array[i];
            array[i] = temp;
        }
    }

    public static T GetRandomElement<T>(this List<T> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    /// <summary>
    /// 通过使用外需
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetList"></param>
    /// <param name="sortingArray"></param>
    public static void ListSortByOuter<T>(T[] targetList, float[] sortingArray, int start)
    {
        //QuickSortFunction(sortingArray, 0, sortingArray.Length - 1, targetList);
        QuickSortObject<T> sortMethod = new QuickSortObject<T>(targetList);
        sortMethod.SetStartAndEnd(start);
        sortMethod.QuickSort(sortingArray, 0, sortingArray.Length - 1);
    }
    /// <summary>
    /// 开区间
    /// </summary>
    public static float[] GetVectorProjection(Vector3[] targetPositions, Vector3 faceTo, int start, int end)
    {
        float[] lines = new float[end - start];
        Vector3 targetPosition;
        faceTo.y = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            //Debug.Log(i);
            //Debug.Log(start);
            targetPosition = targetPositions[i + start];
            targetPosition.y = 0;
            lines[i] = faceTo.ProjectLength(targetPosition);
        }
        return lines;
    }

    public static T GetHashSetFirst<T>(HashSet<T> set)
    {
        foreach (var item in set)
        {
            return item;
        }
        return default;
    }

    public static List<T> GetHashSetItems<T>(this HashSet<T> set, int maxNum, Func<T, bool> condition = null)
    {
        List<T> result = new List<T>(maxNum);
        foreach (T item in set)
        {
            if (condition != null && !(condition.Invoke(item))) continue;
            maxNum--;
            result.Add(item);
            if (maxNum == 0) return result;
        }
        return result;
    }

    public static void Swap<T>(this List<T> list, int a, int b)
    {
        T temp = list[a];
        list[a] = list[b];
        list[b] = temp;
    }

    public static void ListShowFit<T, V>(List<T> slots, List<V> datas, GameObject originSlot, Transform content,Action<T,V> fitAction) where T : MonoBehaviour
    {
        for (int i = slots.Count; i < datas.Count; i++)
        {
            var g = GameObject.Instantiate(originSlot, content);
            slots.Add(g.GetComponent<T>());
        }
        for (int i = datas.Count; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < datas.Count; i++)
        {
            fitAction?.Invoke(slots[i], datas[i]);
            slots[i].gameObject.SetActive(true);
        }
    }

    public static void ListShowFit<T, V>(List<T> slots, V[] datas, GameObject originSlot, Transform content,Action<T,V> fitAction) where T : MonoBehaviour
    {
        if (datas == null)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].gameObject.SetActive(false);
            }
            return;
        }
        for (int i = slots.Count; i < datas.Length; i++)
        {
            var g = GameObject.Instantiate(originSlot, content);
            slots.Add(g.GetComponent<T>());
        }
        for (int i = datas.Length; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < datas.Length; i++)
        {
            fitAction?.Invoke(slots[i], datas[i]);
            slots[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 链表反转
    /// TODO：性能不佳，临时用着
    /// </summary>
    public static void LinkedListReverse<T>(LinkedList<T> target)
    {
        var head = target.First;
        while (head.Next != null)
        {
            var next = head.Next;
            target.Remove(next);
            target.AddFirst(next.Value);
        }
    }
    public static float[,] Combine(params float[][,] array)
    {
        int x = array[0].GetLength(0);
        int y = array[0].GetLength(1);
        float[,] result = new float[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < array.Length; k++)
                {
                    result[i, j] += array[k][i, j];
                }
                result[i, j] /= (float)array.Length;
            }
        }
        return result;
    }
}

/// <summary>
/// 泛型快排
/// </summary>
public class QuickSortObject<T>
{
    T[] changeArray;
    int start;
    public QuickSortObject(T[] array)
    {
        changeArray = array;
    }
    public void SetStartAndEnd(int start)
    {
        this.start = start;
    }


    public void QuickSortFunction(float[] array, int low, int high)
    {
        int keyValuePosition;   //记录关键值的下标
                                //当传递的目标数组含有两个以上的元素时，进行递归调用。（即：当传递的目标数组只含有一个元素时，此趟排序结束）
        if (low < high)
        {
            keyValuePosition = keyValuePositionFunction(array, low, high);  //获取关键值的下标（快排的核心）
            QuickSortFunction(array, low, keyValuePosition - 1);    //递归调用，快排划分出来的左区间
            QuickSortFunction(array, keyValuePosition + 1, high);   //递归调用，快排划分出来的右区间
        }
    }

    //快速排序的核心部分：确定关键值在数组中的位置，以此将数组划分成左右两区间，关键值游离在外。（返回关键值应在数组中的下标）
    private int keyValuePositionFunction(float[] array, int low, int high)
    {
        int leftIndex = low;        //记录目标数组的起始位置（后续动态的左侧下标）
        int rightIndex = high;      //记录目标数组的结束位置（后续动态的右侧下标）

        float keyValue = array[low];  //数组的第一个元素作为关键值
        float temp;
        T tempT;
        //当 （左侧动态下标 == 右侧动态下标） 时跳出循环
        while (leftIndex < rightIndex)
        {
            while (leftIndex < rightIndex && array[leftIndex] >= keyValue)  //左侧动态下标逐渐增加，直至找到小于keyValue的下标
            {
                leftIndex++;
            }
            while (leftIndex < rightIndex && array[rightIndex] < keyValue)  //右侧动态下标逐渐减小，直至找到大于的下标
            {
                rightIndex--;
            }
            if (leftIndex > rightIndex)//如果leftIndex < rightIndex，则交换左右动态下标所指定的值；当leftIndex==rightIndex时，跳出整个循环
            {
                //发生交换
                temp = array[leftIndex];
                array[leftIndex] = array[rightIndex];
                array[rightIndex] = temp;
                //对操作对照数组进行交换
                tempT = changeArray[leftIndex + start];
                changeArray[leftIndex + start] = changeArray[rightIndex + start];
                changeArray[rightIndex + start] = tempT;
            }
        }
        //当左右两个动态下标相等时（即：左右下标指向同一个位置），此时便可以确定keyValue的准确位置
        temp = keyValue;
        tempT = changeArray[low + start];
        if (temp > array[rightIndex])//当keyValue < 左右下标同时指向的值，将keyValue与rightIndex - 1指向的值交换，并返回rightIndex - 1
        {
            array[low] = array[rightIndex - 1];
            array[rightIndex - 1] = temp;

            changeArray[low + start] = changeArray[rightIndex - 1 + start];
            changeArray[rightIndex - 1 + start] = tempT;

            return rightIndex - 1;
        }
        else //当keyValue >= 左右下标同时指向的值，将keyValue与rightIndex指向的值交换，并返回rightIndex
        {
            array[low] = array[rightIndex];
            array[rightIndex] = temp;

            changeArray[low + start] = changeArray[rightIndex + start];
            changeArray[rightIndex + start] = tempT;
            return rightIndex;
        }
    }


    public void QuickSort(float[] arr, int high, int low)
    {
        int i, j;
        float temp;
        i = high;//高端下标  
        j = low;//低端下标  
        temp = arr[i];//取第一个元素为标准元素。  
        T tempT = changeArray[i + start];

        while (i < j)
        {//递归出口是 low>=high  
            while (i < j && temp > arr[j])//后端比temp小，符合降序，不管它，low下标前移
                j--;//while完后指比temp大的那个
            if (i < j)
            {
                changeArray[i + start] = changeArray[j + start];
                arr[i] = arr[j];
                i++;
            }
            while (i < j && temp < arr[i])
                i++;
            if (i < j)
            {
                changeArray[j + start] = changeArray[i + start];
                arr[j] = arr[i];
                j--;
            }
        }//while完，即第一盘排序  
        changeArray[i + start] = tempT;
        arr[i] = temp;//把temp值放到它该在的位置。  

        if (high < i) //注意，下标值	 
            QuickSort(arr, high, i - 1);//对左端子数组递归  
        if (i < low)  //注意，下标值
            QuickSort(arr, i + 1, low);//对右端子数组递归  ；对比上面例子，其实此时i和j是同一下标!!!!!!!!!!!!!
    }

}
