using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger4 : MonoBehaviour
{
    public GameObject trigger5;
    public GameObject wall1;
    public GameObject wall2;
    public bool destory = false;
    public bool trigger1 = true;
    public bool trigger2 = true;
    public bool trigger3 = true;

    TutorialPanel tipPanel;
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowUI("TutorialPanel", (ui) =>
        {
            tipPanel = (ui as TutorialPanel);
        });

        StartCoroutine(SetWallFalse());
    }

    // Update is called once per frame
    void Update()
    {
        if (destory)
            Destroy(gameObject);


        if (UnitControlManager.instance.targetCommandDic["Wolf3"].aliveCount == 0 && trigger2)
        {
            tipPanel.SetShow(5);

            //移除空气墙 刷新地面 开放a d移动镜头
            //wall1.SetActive(false);
            //wall2.SetActive(false);
            BattleManager.instance.UpdateNavMesh();
            //开放镜头移动 开放鼠标右键移动
            LeapSlashImpact.CannotLeappos = default;
            //UI 提示 复活的部队 只能到这了 
            StartCoroutine(ActiveUI());
            //杀死 复活的部队
            trigger2 = false;
        }

    }

    IEnumerator ActiveUI()
    {
        yield return null;


        yield return new WaitForSeconds(1f);

        trigger5.SetActive(true);
        destory = true;



    }
    IEnumerator SetWallFalse()
    {

        yield return new WaitForSeconds(2f);
        wall1.SetActive(false);
        wall2.SetActive(false);
    }
}
