using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI显示进度条
/// </summary>
public class WorkUIBar : MonoBehaviour
{
    public Image workUIIcon;
    public Image workUIIconBg;
    public Image progressBar;
    //public Text 
    private string lastIDName;

    public void UpdateUI(Work work)
    {
        if (lastIDName != work.idName)
        {
            lastIDName = work.idName;
            workUIIconBg.sprite = DataBaseManager.Instance.GetSpriteByIdName(work.idName);
            workUIIcon.sprite = DataBaseManager.Instance.GetSpriteByIdName(work.idName);
        }
        if (work.workNeedNum > 0)
        {
            workUIIcon.fillAmount = (float)work.workingNum / (float)work.workNeedNum;
        }
        else
        {
            workUIIcon.fillAmount = 1f;
        }
        if (work.workload > 0)
            progressBar.fillAmount = work.workProcess / (float)work.workload;
    }

}
