using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGroup : MonoBehaviour
{
    public string uiGroupName;
    private void Awake()
    {
        UIManager.Instance.RegisterUIGroup(transform, uiGroupName);
    }
}
