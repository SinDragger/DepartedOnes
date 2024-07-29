using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int buildingCode;

    public List<int> GetBuildingAble()
    {
        List<int> result = new List<int>();
        if (buildingCode == 1)
            result.Add(12);
        return result;
    }
}
