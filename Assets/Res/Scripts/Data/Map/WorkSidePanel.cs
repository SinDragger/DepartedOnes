using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkSidePanel : MonoBehaviour
{
    public Text refugeeNumber;
    public Button refugeeButton;
    public Button cancelButton;
    System.Action workStartCallback;
    System.Action workCancelCallback;
    Work nowWork;
    private void Awake()
    {
        refugeeButton.SetBtnEvent(OnWorkButtonClick);
        cancelButton.SetBtnEvent(OnEmptyButtonClick);
    }
    public void Init(Work work, System.Action workStartCallback, System.Action workCancelCallback)
    {
        nowWork = work;
        this.workStartCallback = workStartCallback;
        this.workCancelCallback = workCancelCallback;
        gameObject.SetActive(true);
        Update();
    }

    public void Update()
    {
        refugeeNumber.text = $"{RefugeeManager.Instance.ableRefugeeNum}({RefugeeManager.Instance.refugeeNumber})";
    }

    public void OnWorkButtonClick()
    {
        if (!RefugeeManager.Instance.WorkIsWorking(nowWork))
        {
            if(RefugeeManager.Instance.ableRefugeeNum <= 0)
            {
                //弹出人数不足
                return;
            }
            RefugeeManager.Instance.DispatchToWork(nowWork);
            workStartCallback?.Invoke();
            gameObject.SetActive(false);
        }
    }
    public void OnEmptyButtonClick()
    {
        if (RefugeeManager.Instance.WorkIsWorking(nowWork))
        {
            RefugeeManager.Instance.GiveUpWork(nowWork);
            workCancelCallback?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
