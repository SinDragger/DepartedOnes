using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 系统设置
/// </summary>
public class SystemSettingManager : Singleton<SystemSettingManager>
{
    public string language;

    protected override void Init()
    {
        if (PlayerPrefs.HasKey("Setting.Language"))
        {
            language = PlayerPrefs.GetString("Setting.Language");
        }
        else
        {
            language = Application.systemLanguage.ToString();
        }
        //加载本地的数据存储
        base.Init();
    }
}
