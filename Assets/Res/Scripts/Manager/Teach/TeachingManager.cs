using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TeachingManager : MonoSingleton<TeachingManager>, ITutorialScene
{
    InputResponseNode node;

    public GameObject pos1;

    public GameObject pos2;

    public GameObject pos3;

    public GameObject pos4;


    protected override void Awake()
    {
        base.Awake();
        BattleManager.instance.isNonBattleMap = true;
    }
    public List<GameObject> positionObjects;

    public Vector3 GetPosition(string target)
    {
        GameObject gameObject = positionObjects.Find((o) => o.name == target);
        if (!gameObject) return default;
        return gameObject.transform.position;
    }

    public void PlayTutorial()
    {

        node = new InputResponseNode(BlockInput, 5);
        GameManager.instance.playerForce.SetLimitedRes(Constant_AttributeString.RES_SOULPOINT, 1000);

        //启动ARPG模式 设置UI
        //BlockSkill1();
        //BlockSkill2();
        //BlockRightUp();
        //BlockCameraMove();
        BlockTab();
        BlockMIDDLEDOWN();
        //if (GameManager.instance.defaultARPG && ARPGManager.Instance.SetCurrentGeneralControl())
        //{

        //    ARPGManager.Instance.Active();
        //    GameManager.instance.defaultARPG = false;
        //    //var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;
        //    //SkillListUI.showPanel.SetActive(false);
        //}
        BattleManager.instance.UpdateNavMesh();

        CameraControl.Instance.SetCameraToMap();
        CameraControl.Instance.xMax = -10;
        CameraControl.Instance.yMax = -50;
        CameraControl.Instance.SetToTargetLevel(0);
        //CameraControl.Instance.rotateCenter.transform.position = ARPGManager.Instance.generals.DictionaryFirst().transform.position;
        CloseGb();

    }

    public void CloseGb()
    {
        pos1.SetActive(false);
        //pos2.SetActive(false);
        //pos3.SetActive(false);

    }

    protected bool BlockInput(object param)
    {
        return true;
    }
    public void BlockSkill1()
    {
        var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;
        InputManager.Instance.keyResponseChain.RegistInputResponse(InputType.PLAYER_SKILL1.ToString(), node);
    }
    public void BlockSkill2()
    {
        var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;

        InputManager.Instance.keyResponseChain.RegistInputResponse(InputType.PLAYER_SKILL2.ToString(), node);
    }
    public void BlockRightUp()
    {
        InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.RIGHT_UP, node);
    }
    public void BlockMIDDLEDOWN()
    {

        //InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.MIDDLE_DOWN, node);
        //InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.MIDDLE, node);
        //InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.MIDDLE_UP, node);
        InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.SCROLL, node);



    }
    public void BlockCameraMove()
    {

        InputManager.Instance.keyResponseChain.RegistInputResponse(InputType.CAMERA_LEFT.ToString(), node);
        InputManager.Instance.keyResponseChain.RegistInputResponse(InputType.CAMERA_UP.ToString(), node);
        InputManager.Instance.keyResponseChain.RegistInputResponse(InputType.CAMERA_RIGHT.ToString(), node);
        InputManager.Instance.keyResponseChain.RegistInputResponse(InputType.CAMERA_DOWN.ToString(), node);
        InputManager.Instance.keyDownResponseChain.RegistInputResponse(InputType.CAMERA_ROTATE_LEFT.ToString(), node);
        InputManager.Instance.keyDownResponseChain.RegistInputResponse(InputType.CAMERA_ROTATE_RIGHT.ToString(), node);
    }


    public void BlockTab()
    {
        InputManager.Instance.keyUpResponseChain.RegistInputResponse(InputType.TAB.ToString(), node);
    }
    public void ActiveMIDDLEDOWN()
    {
        //InputManager.Instance.mouseResponseChain.RemoveInputResponse(MouseCode.MIDDLE_DOWN, node);
        //InputManager.Instance.mouseResponseChain.RemoveInputResponse(MouseCode.MIDDLE, node);
        //InputManager.Instance.mouseResponseChain.RemoveInputResponse(MouseCode.MIDDLE_UP, node);
        InputManager.Instance.mouseResponseChain.RemoveInputResponse(MouseCode.SCROLL, node);
    }

    public void ActiveTab()
    {
        InputManager.Instance.keyUpResponseChain.RemoveInputResponse(InputType.TAB.ToString(), node);

    }

    public void ActiveRightUp()
    {
        InputManager.Instance.mouseResponseChain.RemoveInputResponse(MouseCode.RIGHT_UP, node);

    }
    public void ActiveSkill1()
    {
        var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;
        SkillListUI.showPanel.SetActive(true);
        InputManager.Instance.keyResponseChain.RemoveInputResponse(InputType.PLAYER_SKILL1.ToString(), node);
    }
    public void ActiveSkill2()
    {
        var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;
        SkillListUI.showPanel.SetActive(true);
        InputManager.Instance.keyResponseChain.RemoveInputResponse(InputType.PLAYER_SKILL2.ToString(), node);
    }


    //替换角色死亡时执行的方法
    void OnDie()
    {
        ARPGManager.Instance.nowTarget.nowHp = 200;


    }
    public void BlockLeft()
    {

        InputManager.Instance.keyResponseChain.RegistInputResponse(InputType.PLAYER_SKILL1.ToString(), node);
    }
    private void Update()
    {
        //if (GameManager.instance.defaultARPG && ARPGManager.Instance.SetCurrentGeneralControl())
        //{

        //    ARPGManager.Instance.Active();
        //    GameManager.instance.defaultARPG = false;
        //    var SkillListUI = UIManager.Instance.GetUI("ARPGSkillListUI") as ARPGSkillListUI;
        //    SkillListUI.showPanel.SetActive(false);
        //}
    }

    public void UpdateNav()
    {
        BattleManager.instance.UpdateNavMesh();
    }

    private void OnDisable()
    {
        ActiveRightUp();
        ActiveSkill1();
        ActiveSkill2();
        ActiveTab();
        ActiveMIDDLEDOWN();
    }
}
