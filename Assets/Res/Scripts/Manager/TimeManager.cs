using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 时间管理-小时/日/月/周。最少用0.1小时来表达
/// </summary>
public class TimeManager : Singleton<TimeManager>
{
    AdjustDateTime now = new AdjustDateTime(0);
    public AdjustDateTime Now => now;
    TimeManager_InputModule inputModule = new TimeManager_InputModule();
    public int nowSpeedLevel = 3;
    float nowDeltaMillisecondTime;
    int prevTimeSpeedLevel;
    public float nowDeltaTime => nowDeltaMillisecondTime / 1000f;

    List<(float, Action)> realTimeCycleList = new List<(float, Action)>();
    protected override void Init()
    {
        inputModule.Init();
        inputModule.Active();
        base.Init();
    }

    float[] speedLevel =
    {
        //战斗内的时间-[3]
        1f,//1s=1s
        //游戏外的时间-[6]
        60f,//1s = 1min
        120f,//1s = 2min
        240f,//1s = 4min
        //480f,//1s = 8min
        960f,//1s = 16min
        //1920f,//1s = 32min
        3840f,//1s = 64min
    };
    float[] battleSpeedLevel =
    {
        0.1f,//10s=1s
        0.25f,//4s=1s
        0.5f,//2s=1s
        1f,//1s=1s
        2f,//1s=2s
        4f,//1s=4s
    };

    public void RegistRealTimeCycleAction(float timeSpace, Action callback)
    {
        realTimeCycleList.Add((timeSpace, callback));
    }

    public void RemoveRealTimeCycleAction(Action callback)
    {
        realTimeCycleList.RemoveAll((pair) => pair.Item2 == callback);
    }

    public void AddTimeSpeed()
    {
        if (isStop) return;
        nowSpeedLevel++;
        if (nowSpeedLevel >= speedLevel.Length - 2) nowSpeedLevel = speedLevel.Length - 3; //封住上限
    }
    public void MinusTimeSpeed()
    {
        if (isStop) return;
        nowSpeedLevel--;
        if (nowSpeedLevel < 0) nowSpeedLevel = 0;
    }
    public void MaxTimeSpeed()
    {
        if (isStop) return;
        //判断是否在战场之中
        nowSpeedLevel = speedLevel.Length - 1;
    }
    public void MinTimeSpeed()
    {
        nowSpeedLevel = 0;
    }
    public void SwitchTimeSpeed()
    {
        if (isStop) return;
        if (nowSpeedLevel != 0)
        {
            prevTimeSpeedLevel = nowSpeedLevel;
            nowSpeedLevel = 0;
        }
        else
        {
            nowSpeedLevel = prevTimeSpeedLevel;
            prevTimeSpeedLevel = 0;
        }
    }

    bool isStop;

    public void SetToStop()
    {
        if (nowSpeedLevel != 0)
        {
            prevTimeSpeedLevel = nowSpeedLevel;
            nowSpeedLevel = 0;
        }
        isStop = true;
    }
    public void SetToRecovery()
    {
        if (nowSpeedLevel == 0)
        {
            nowSpeedLevel = prevTimeSpeedLevel;
            prevTimeSpeedLevel = 0;
        }
        isStop = false;
    }

    float realTimeCount;
    public bool tempMaxSpeed;
    public bool tempMaxSpeedProtect;

    public void SetTempMaxSpeedProtect()
    {
        tempMaxSpeedProtect = true;
        tempMaxSpeed = false;
    }

    /// <summary>
    /// 未暂停的情况调用时间更新
    /// </summary>
    public void UpdateTime(float realTime)
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            tempMaxSpeedProtect = false;
        }
        if (Input.GetKey(KeyCode.LeftAlt) && !isStop)
        {
            if (!tempMaxSpeedProtect)
                tempMaxSpeed = true;
        }
        else
        {
            tempMaxSpeed = false;
        }
        if (tempMaxSpeed)
        {
            nowDeltaMillisecondTime = realTime * 1000 * speedLevel[speedLevel.Length - 2];
        }
        else
        {
            nowDeltaMillisecondTime = realTime * 1000 * speedLevel[nowSpeedLevel];
        }
        now.AddMilliseconds((int)(nowDeltaMillisecondTime));
        for (int i = 0; i < realTimeCycleList.Count; i++)
        {
            if ((int)(realTimeCount / realTimeCycleList[i].Item1) != (int)((realTimeCount + realTime) / realTimeCycleList[i].Item1))
            {
                realTimeCycleList[i].Item2.Invoke();
            }
        }
        realTimeCount += realTime;
    }

    void TriggerSecondChange()
    {

    }

    void TriggerMinuteChange()
    {

    }

    void TriggerHourChange()
    {

    }

    void TriggerDayChange()
    {

    }

    void TriggerWeekChange()
    {

    }

    void TriggerMonthChange()
    {

    }

    void TriggerYearChange()
    {

    }
}

public interface ITimeUpdatable
{
    void OnUpdate(float deltaTime);
}

/// <summary>
/// 一月28天4周
/// </summary>
public class AdjustDateTime : IComparable
{
    public const long TicksPerSecond = 1000;
    /// <summary>
    /// 总共时间长度
    /// </summary>
    public long ticks;

    public static readonly DateTime MaxValue;
    public static readonly DateTime MinValue;

    public AdjustDateTime(long ticks)
    {
        this.ticks = ticks;
    }

    public AdjustDateTime(int year, int month, int day)
    {
        ticks = ((year * 12 + month) * 28 + day) * 24 * 60 * 60 * TicksPerSecond;
    }

    public AdjustDateTime(int year, int month, int day, int hour, int minute, int second)
    {
        ticks = (((((year * 12 + month) * 28 + day) * 24 + hour) * 60 + minute) * 60 + second) * TicksPerSecond;

    }

    public int Millisecond
    {
        get
        {
            return (int)(ticks % 1000);
        }
    }
    public int Second
    {
        get
        {
            return ((int)(ticks / 1000)) % 60;
        }
    }
    public int Minute
    {
        get
        {
            return ((int)(ticks / 1000 / 60)) % 60;
        }
    }
    public int Hour
    {
        get
        {
            return ((int)(ticks / 1000 / 60 / 60)) % 24;
        }
    }
    public int Day
    {
        get
        {
            return ((int)(ticks / 1000 / 60 / 60 / 24)) % 7;
        }
    }
    public int Week
    {
        get
        {
            return ((int)(ticks / 1000 / 60 / 60 / 24 / 7)) % 4;
        }
    }
    public int Month
    {
        get
        {
            return ((int)(ticks / 1000 / 60 / 60 / 24 / 7 / 4)) % 12;
        }
    }
    public int Year
    {
        get

        {
            return ((int)(ticks / 1000 / 60 / 60 / 24 / 7 / 4 / 12));
        }
    }

    public float DayPercent
    {
        get
        {
            int dayTicks = (1000 * 60 * 60 * 24);

            return (float)(ticks % dayTicks) / (float)dayTicks;
        }
    }

    public int DayOfYear { get; }
    public int DayOfMonth { get; }
    public TimeSpan TimeOfDay { get; }

    public int Compare(AdjustDateTime t1, AdjustDateTime t2)
    {
        return t1.ticks.CompareTo(t2.ticks);
    }
    public bool Equals(AdjustDateTime t1, AdjustDateTime t2)
    {
        return t1.ticks == t2.ticks;
    }
    public AdjustDateTime AddMilliseconds(int value)
    {
        ticks += value;
        return this;
    }
    public AdjustDateTime AddSeconds(int value)
    {
        ticks += value * 1000;
        return this;
    }
    public AdjustDateTime AddMinutes(int value)
    {
        ticks += value * 1000 * 60;
        return this;
    }
    public AdjustDateTime AddHours(int value)
    {

        ticks += value * 1000 * 60 * 60;
        return this;
    }
    public AdjustDateTime AddDays(int value)
    {
        ticks += value * 1000 * 60 * 60 * 24;
        return this;
    }
    public AdjustDateTime AddWeeks(int value)
    {
        ticks += value * 1000 * 60 * 60 * 24 * 7;
        return this;
    }
    public AdjustDateTime AddMonths(int value)
    {
        ticks += value * 1000 * 60 * 60 * 24 * 7 * 4;
        return this;
    }
    public AdjustDateTime AddYears(int value)
    {
        ticks += value * 1000 * 60 * 60 * 24 * 7 * 4 * 12;
        return this;
    }

    public int CompareTo(object obj)
    {
        return 0;
    }

    public int CompareTo(AdjustDateTime obj)
    {
        return ticks.CompareTo(obj.ticks);
    }
}