using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager instance;

    private List<FutureBehaviour> futureCoroutineList = new List<FutureBehaviour>();
    // Start is called before the first frame update

    public static void DeleteCoroutine(FutureBehaviour coroutine)
    {
        if (coroutine == null) return;
        if (instance.futureCoroutineList != null && instance.futureCoroutineList.Contains(coroutine))
            instance.futureCoroutineList.Remove(coroutine);
    }

    public static void StartDelayedCoroutine(float time, Action action)
    {
        if (action == null) return;
        CoroutineManager manager = GetInstance();
        if (time <= 0f) { action.Invoke(); return; }
        instance.AddNewFutureCoroutine(new FutureBehaviour(time, action));
    }

    public static FutureBehaviour DelayedCoroutine(float time, Action action)
    {
        if (action == null) return null;
        CoroutineManager manager = GetInstance();
        if (time <= 0f) { action.Invoke(); return null; }
        var corountine = new FutureBehaviour(time, action);
        instance.AddNewFutureCoroutine(corountine);
        return corountine;
    }

    public static void TriggerAllCoroutine()
    {
        CoroutineManager manager = GetInstance();
        manager.TriggerAllFutureCoroutine();
    }

    public static void RemoveCoroutine(Action action)
    {
        if (action == null) return;
        CoroutineManager manager = GetInstance();
        instance.RemoveFutureCoroutine(action);
    }

    public static void StartFrameDelayedCoroutine(Action action)
    {
        if (action == null) return;
        CoroutineManager manager = GetInstance();
        instance.StartCoroutine(instance.WaitFrameAction(action));
    }

    private float GetMinStartTime()
    {
        return 0.3f;
    }
    private void AddNewFutureCoroutine(FutureBehaviour coroutine)
    {
        if (coroutine == null) return;
        for (int i = 0; i < futureCoroutineList.Count; i++)
        {
            if (futureCoroutineList[i].waitTime >= coroutine.waitTime)
            {
                futureCoroutineList[i].waitTime -= coroutine.waitTime;
                futureCoroutineList.Insert(i, coroutine);
                return;
            }
            else
            {
                coroutine.waitTime -= futureCoroutineList[i].waitTime;
            }
        }
        futureCoroutineList.Add(coroutine);
    }
    private void RemoveFutureCoroutine(Action action)
    {
        futureCoroutineList.RemoveAll((f) => f.futureBehaviour == action);
    }
    private void TriggerAllFutureCoroutine()
    {
        for (int i = 0; i < futureCoroutineList.Count; i++)
        {
            futureCoroutineList[i].futureBehaviour.Invoke();
        }
        futureCoroutineList.Clear();
    }
    float nowClipTime;
    private void CheckFutureCoroutines()
    {
        if (futureCoroutineList.Count == 0) return;
        nowClipTime += Time.fixedDeltaTime;
        while (futureCoroutineList.Count != 0 && nowClipTime != 0f)
        {
            if (futureCoroutineList[0] == null) { futureCoroutineList.Remove(futureCoroutineList[0]); }
            else
            {
                if (futureCoroutineList[0].waitTime < nowClipTime)
                {
                    Action nowActiveAction = futureCoroutineList[0].futureBehaviour;
                    nowClipTime -= futureCoroutineList[0].waitTime;
                    futureCoroutineList.Remove(futureCoroutineList[0]);
                    nowActiveAction.Invoke();
                    //nowActiveAction?.Invoke();
                }
                else
                {
                    futureCoroutineList[0].waitTime -= nowClipTime;
                    nowClipTime = 0f;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        CheckFutureCoroutines();
    }

    private static CoroutineManager GetInstance()
    {
        if (instance == null)
        {
            //UnityUtils.Log("Create Coroutine Manager");
            var gameObject = new GameObject("CoroutineManager");
            // Hide the object so that it it won't be accessible (to make sure it won't be deleted accidentally).
            gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            gameObject.AddComponent<CoroutineManager>();
            instance = gameObject.GetComponent<CoroutineManager>();
            //DontDestroyOnLoad(gameObject);
        }
        return instance;
    }
    public static void StartWaitUntil(Func<bool> waitCheck, Action completeAction)
    {
        if (waitCheck.Invoke())
        {
            completeAction.Invoke();
        }
        else
        {
            GetInstance().StartCoroutine(instance.WaitUntilComplete(waitCheck, completeAction));
        }
    }

    IEnumerator WaitForAction(float time, Action action)
    {
        if (time > 0f)
        {
            yield return new WaitForSeconds(time);
        }
        action?.Invoke();
    }

    IEnumerator WaitFrameAction(Action action)
    {
        yield return null;
        action.Invoke();
        // action?.Invoke();
    }
    IEnumerator WaitUntilComplete(Func<bool> waitCheck, Action completeAction)
    {
        yield return new WaitUntil(waitCheck);
        //action.GetActionWithException().Invoke();
        completeAction?.Invoke();
    }

}

public class FutureBehaviour
{
    public float waitTime;//排队的时间长度
    public Action futureBehaviour;

    public FutureBehaviour(float waitTime, Action futureBehaviour)
    {
        this.waitTime = waitTime;
        this.futureBehaviour = futureBehaviour;
    }
}

public class BehaviourChain
{
    private List<FutureBehaviour> futureBehaviourList = new List<FutureBehaviour>(100);
    public float nowTime;
    public void DeleteCoroutine(FutureBehaviour coroutine)
    {
        if (coroutine == null) return;
        if (futureBehaviourList != null && futureBehaviourList.Contains(coroutine))
            futureBehaviourList.Remove(coroutine);
    }

    public void PushDelayAction(float time, Action action)
    {
        if (action == null) return;
        if (time <= 0f) { action.Invoke(); return; }
        AddNewFutureBehaviour(new FutureBehaviour(time, action));
    }

    public void TriggerAllBehaviour()
    {
        TriggerAllFutureBehaviour();
    }

    public void RemoveBehaviour(Action action)
    {
        if (action == null) return;
        RemoveFutureCoroutine(action);
    }

    private void AddNewFutureBehaviour(FutureBehaviour coroutine)
    {
        if (coroutine == null) return;
        for (int i = 0; i < futureBehaviourList.Count; i++)
        {
            if (futureBehaviourList[i].waitTime >= coroutine.waitTime)
            {
                futureBehaviourList[i].waitTime -= coroutine.waitTime;
                futureBehaviourList.Insert(i, coroutine);
                return;
            }
            else
            {
                coroutine.waitTime -= futureBehaviourList[i].waitTime;
            }
        }
        futureBehaviourList.Add(coroutine);
    }
    private void RemoveFutureCoroutine(Action action)
    {
        futureBehaviourList.RemoveAll((f) => f.futureBehaviour == action);
    }
    private void TriggerAllFutureBehaviour()
    {
        for (int i = 0; i < futureBehaviourList.Count; i++)
        {
            futureBehaviourList[i].futureBehaviour.Invoke();
        }
        futureBehaviourList.Clear();
    }
    float nowClipTime;
    public void CheckFutureBehaviours(float passTime)
    {
        if (futureBehaviourList.Count == 0) return;
        nowClipTime += passTime;
        while (futureBehaviourList.Count != 0 && nowClipTime != 0f)
        {
            if (futureBehaviourList[0] == null) { futureBehaviourList.Remove(futureBehaviourList[0]); }
            else
            {
                if (futureBehaviourList[0].waitTime < nowClipTime)
                {
                    Action nowActiveAction = futureBehaviourList[0].futureBehaviour;
                    nowClipTime -= futureBehaviourList[0].waitTime;
                    nowTime += futureBehaviourList[0].waitTime;
                    futureBehaviourList.Remove(futureBehaviourList[0]);
                    nowActiveAction.Invoke();
                    //nowActiveAction?.Invoke();
                }
                else
                {
                    futureBehaviourList[0].waitTime -= nowClipTime;
                    nowTime += nowClipTime;
                    nowClipTime = 0f;
                }
            }
        }
    }
}