using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Trigger1 : MonoBehaviour
{
    public GameObject trigger2;
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
    TutorialPanel tipPanel;
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowUI("TutorialPanel", (ui) =>
        {
            tipPanel = (ui as TutorialPanel);
        });
       tipPanel.SetShow(2);
    }

    // Update is called once per frame
    void Update()
    {
        //if (destory)
        //    Destroy(gameObject);

        if (Ttransform != null && Vector3.Distance(Ttransform.position, transform.position) > 10f)
        {
            
            //  TeachingManager.instance.ActiveSkill1();
            UnitControlManager.instance.targetCommandDic["Wolf1"].SetTarget(ARPGManager.Instance.currentGeneralControl.m_SoldierModel.commander); 
            destory = true;
        }

    }

}
