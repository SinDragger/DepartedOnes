using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorMapDataSave : MonoBehaviour
{
    public void OnClick()
    {
        SectorBlockManager.Instance.PrintMapData();
    }
}
