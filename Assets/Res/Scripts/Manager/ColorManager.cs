using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : Singleton<ColorManager>
{
    public Dictionary<string, Color> colorDic;
    protected override void Init()
    {
        colorDic = new Dictionary<string, Color>();
        colorDic[UnitType.WORKER.ToString()]= new Color(179f / 255f, 159f / 255f, 23f / 255f, 1f);
        colorDic[UnitType.FIGHTER.ToString()]= new Color(156f / 255f, 25f / 255f, 4f / 255f, 1f);
        colorDic[UnitType.CASTER.ToString()]= new Color(4f / 255f, 115f / 255f, 156f / 255f, 1f);
        colorDic[UnitType.MONSTER.ToString()]= new Color(54f / 255f, 115f / 255f, 156f / 255f, 1f);
        base.Init();
    }

    public Color GetColor(string key)
    {
        if (colorDic.ContainsKey(key))
        {
            return colorDic[key];
        }
        return default;
    }

}
