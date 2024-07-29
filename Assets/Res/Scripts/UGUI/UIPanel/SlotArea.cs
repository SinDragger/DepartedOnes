using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotArea : MonoBehaviour
{
    public Transform content;
    public GameObject slotPrefab;
    public void Init<T,V>(List<T> slotList, List<V> dataList,System.Action<T,V> action) where T : MonoBehaviour
    {
        ArrayUtil.ListShowFit(slotList, dataList, slotPrefab, content, action);
    }
}
