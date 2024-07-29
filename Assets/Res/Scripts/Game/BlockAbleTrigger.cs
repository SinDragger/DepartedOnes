using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAbleTrigger : MonoBehaviour
{
    /// <summary>
    /// 参数
    /// </summary>
    public object[] param;
    public System.Action onBlockAction;
    public GameObject checkTarget;
    private void OnTriggerEnter(Collider other)
    {
        var blockTrigger = other.GetComponent<IBlockTriggerable>();
        if (blockTrigger != null)
        {
            onBlockAction?.Invoke();
            blockTrigger.OnBlock(param);
            checkTarget.transform.SetParent(other.transform);
        }
    }
}

/// <summary>
/// 实际为伤害类型所触发的内容
/// </summary>
public interface IBlockTriggerable
{
    void OnBlock(params object[] param);
}