using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Trigger5 : MonoBehaviour
{
    bool trigger1 = true;
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
        UnitControlManager.instance.targetCommandDic["NPC"].AutoDie();
        if (trigger1 && ARPGManager.Instance.currentGeneralControl.transform.position.x > transform.position.x)
        {
            trigger1 = false;
            CameraControl.Instance.xMax = 50;
            CameraControl.Instance.yMax = 50;
            tipPanel.EndShow();
            GameManager.instance.LeaveArea();
        }
    }
}
