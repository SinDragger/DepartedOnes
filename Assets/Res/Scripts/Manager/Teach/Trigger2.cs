using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2 : MonoBehaviour
{
    public GameObject trigger3;
    public bool destory = false;
    Transform Ttransform
    {
        get
        {
            if (ARPGManager.Instance.currentGeneralControl != null)
                return ARPGManager.Instance.currentGeneralControl.transform;
            else return null;
        }
    }
    // Start is called before the first frame update
    TutorialPanel tipPanel;
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowUI("TutorialPanel", (ui) =>
        {
            tipPanel = (ui as TutorialPanel);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (destory)
            Destroy(gameObject);

        if (UnitControlManager.instance.targetCommandDic["Wolf1"].aliveCount == 0)
        {
            tipPanel.SetShow(3);
            trigger3.SetActive(true);
            //TeachingManager.instance.ActiveSkill2();
            // UnitControlManager.instance.targetCommandDic["Wolf1"].SetTarget(ARPGManager.Instance.currentGeneralControl.m_SoldierModel.commander);
            destory = true;
        }


    }
}
