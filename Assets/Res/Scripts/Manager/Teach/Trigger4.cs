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

            //�Ƴ�����ǽ ˢ�µ��� ����a d�ƶ���ͷ
            //wall1.SetActive(false);
            //wall2.SetActive(false);
            BattleManager.instance.UpdateNavMesh();
            //���ž�ͷ�ƶ� ��������Ҽ��ƶ�
            LeapSlashImpact.CannotLeappos = default;
            //UI ��ʾ ����Ĳ��� ֻ�ܵ����� 
            StartCoroutine(ActiveUI());
            //ɱ�� ����Ĳ���
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
