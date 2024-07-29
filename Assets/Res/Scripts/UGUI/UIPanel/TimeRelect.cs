using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeRelect : MonoBehaviour
{
    public Image[] weekIcons;
    public Image[] monthIcons;
    public Image[] speedIcons;
    public Transform circlePoint;
    public Text monthText;


    private void Update()
    {
        SynTimeManager();
    }

    void SynTimeManager()
    {
        float dayPercent = TimeManager.Instance.Now.DayPercent;
        circlePoint.localEulerAngles = new Vector3(0, 0, 360 * dayPercent);
        for (int i = 0; i < weekIcons.Length; i++)
        {
            weekIcons[i].color = TimeManager.Instance.Now.Day > i ? Color.green : Color.white;
        }
        for (int i = 0; i < monthIcons.Length; i++)
        {
            monthIcons[i].color = TimeManager.Instance.Now.Week > i ? Color.green : Color.white;
        }
        for (int i = 0; i < speedIcons.Length; i++)
        {
            if (TimeManager.Instance.tempMaxSpeed)
            {
                speedIcons[i].gameObject.SetActive(true);
            }
            else
            {
                speedIcons[i].gameObject.SetActive(TimeManager.Instance.nowSpeedLevel >= i);
            }
        }
        monthText.text = GetRomeTime(TimeManager.Instance.Now.Month);
    }

    private string GetRomeTime(int num)
    {
        switch (num)
        {
            case 0: return "I";
            case 1: return "II";
            case 2: return "III";
            case 3: return "IV";
            case 4: return "V";
            case 5: return "VI";
            case 6: return "VII";
            case 7: return "VIII";
            case 8: return "IX";
            case 9: return "X";
            case 10: return "XI";
            case 11: return "XII";
        }
        return "I";
    }
}
