using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFieldMark : MonoBehaviour,ISelectAble
{
    public WarBattle reflectWar;
    [SerializeField]
    CanvasGroup alpha;
    /// <summary>
    /// 周期时间
    /// </summary>
    public float intervalTime;
    float nowTime;
    public float min;
    public float max;
    public void Init(WarBattle warBattle)
    {
        reflectWar = warBattle;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void OnUpdate()
    {

    }

    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = new Vector3(pos.x, pos.y, -4f);
    }


    private void Update()
    {
        nowTime += Time.deltaTime;
        if (nowTime > intervalTime) nowTime -= intervalTime;
        float percent = nowTime / intervalTime;
        percent *= 2;
        if (percent > 1f) percent = 2f - percent;
        alpha.alpha = Mathf.Lerp(min, max, percent);
    }

    public void OnSelect(bool value)
    {
        //选中——返回战斗？
    }

    public void OnUIClick()
    {
        UIManager.Instance.ShowUI("BattleSandTablePanel",(ui)=> {
            (ui as BattleSandTablePanel).Init(reflectWar);
        });
    }
}
