using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMiniMapControlFlag : MonoBehaviour
{
    public CommandUnit sourceCommand;
    public Image icon;
    [HideInInspector]
    public float pointScale;
    bool isDied = false;
    /// <summary>
    /// 测试用
    /// </summary>
    /// <param name="belong"></param>
    public void Init(int belong)
    {
        isDied = false;
        icon.color = GameManager.instance.GetForceColor(belong);
    }
    public void UpdatePos(float scale)
    {
        if (isDied)
        {
            if (sourceCommand.troopState != TroopState.DESTROYED)
            {
                isDied = false;
            }
            return;
        }
        transform.localPosition = new Vector3(sourceCommand.lastPosition.x * scale, sourceCommand.lastPosition.z * scale, 0);
        transform.localScale = Vector3.one * scale * pointScale;
        if (sourceCommand.troopState == TroopState.DESTROYED)
        {
            isDied = true;
            icon.color = Color.gray;
            transform.SetAsFirstSibling();
        }
    }
}
