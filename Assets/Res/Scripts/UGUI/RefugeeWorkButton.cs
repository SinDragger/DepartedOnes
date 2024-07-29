using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefugeeWorkButton : MonoBehaviour
{
    public Image cover;
    public Button button;
    public GameObject refugeeHeadIcon;
    WorkSidePanel sidePanelReflect;
    Work work;
    private void Awake()
    {
        button.SetBtnEvent(OnClick);
    }
    public void Init(Work work, WorkSidePanel sidePanel)
    {
        if (this.work == work) return;
        this.work = work;
        sidePanelReflect = sidePanel;
        if (!work.isComplete)
        {
            gameObject.SetActive(true);
        }
        //向RefugeeManager拿具体情况
        if (RefugeeManager.Instance.WorkIsWorking(work))
        {
            refugeeHeadIcon.SetActive(true);
        }
        else
        {
            refugeeHeadIcon.SetActive(false);
        }
        Update();
    }

    private void Update()
    {
        //向Legion获取所有在其上的
        if (work == null) return;
        cover.fillAmount = work.workProcess / work.workload;
        if (work.isComplete) {
            gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        sidePanelReflect.Init(work, () => { refugeeHeadIcon.SetActive(true); }, () => { refugeeHeadIcon.SetActive(false); });
    }

    private void OnReset()
    {
        work = null;
    }
}
