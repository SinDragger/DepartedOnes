using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Trigger3 : MonoBehaviour
{
    public GameObject trigger4;

    public bool destory = false;
   public ARPGControl aRPGControl;

    private void Awake()
    {
        // LeapSlashImpact.CannotLeappos = transform.position;

    }
    TutorialPanel tipPanel;
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowUI("TutorialPanel", (ui) =>
        {
            tipPanel = (ui as TutorialPanel);
        });
     
        aRPGControl = ARPGManager.Instance.currentGeneralControl;
    }
    // Update is called once per frame
    void Update()
    {
        if (destory)
            Destroy(gameObject);
        if (Vector3.Distance(aRPGControl.transform.position,transform.position)<10)
        {

            tipPanel.SetShow(4);
            ARPGManager.Instance.canShowSkillUI=true;
            aRPGControl.ShowUI();
        }

        if (UnitControlManager.instance.targetCommandDic["NPC"].aliveCount > 0)
        {

            //TeachingManager.instance.ActiveSkill2();
            UnitControlManager.instance.targetCommandDic["Wolf3"].SetTarget(UnitControlManager.instance.targetCommandDic["NPC"]);
            destory = true;
        }

    }
}
