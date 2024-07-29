using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseNextPanel : UIPanel
{
    public override string uiPanelName => "ChooseNextPanel";
    public static string nextMapName;
    public Text countDownText;
    public Button leftChoose;
    public Button centorChoose;
    public Button rightChoose;
    public NextChooseSlot leftChooseSlot;
    public NextChooseSlot centorChooseSlot;
    public NextChooseSlot rightChooseSlot;

    public NextChooseSlot bossChooseSlot;
    public Button bossChoose;

    /// <summary>
    /// 第一层
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();
    }
    public override void OnShow(bool withAnim = true)
    {
        base.OnShow(withAnim);
        int left = 7 - GameManager.instance.nowLayerCount;
        countDownText.text = $"决战倒计时：{left}";
        if (left == 0)
        {
            //显示最终决战
            var mapName = $"Final_Boss";
            bossChooseSlot.Init(mapName);
            bossChoose.SetBtnEvent(() =>
            {
                OnHide();
                //注册目标的id
                //切换到下一场
                if (BattleManager.instance)
                    BattleManager.instance.ClearBattleField();
                nextMapName = mapName;
                GameManager.instance.EnterRogueBattle(1);
            });
            bossChooseSlot.gameObject.SetActive(true);
            leftChooseSlot.gameObject.SetActive(false);
            centorChooseSlot.gameObject.SetActive(false);
            rightChooseSlot.gameObject.SetActive(false);
            return;
        }
        if (left < 0)
        {
            OnHide();
            SceneManager.Instance.BackToStartMenu(0f);
            UIManager.Instance.ShowUI("BattleWinPanel");
            //打完终章战斗胜利
            return;
        }
        int layerCount = GameManager.instance.nowLayerCount;
        if (layerCount % 2 == 0)//精英战
        {
            int number = layerCount / 2;
            var mapName = $"Layer_{number}_Boss";
            bossChooseSlot.Init(mapName);
            bossChoose.SetBtnEvent(() =>
            {
                OnHide();
                //注册目标的id
                //切换到下一场
                if (BattleManager.instance)
                    BattleManager.instance.ClearBattleField();
                nextMapName = mapName;
                GameManager.instance.EnterRogueBattle(1);
            });

            bossChooseSlot.gameObject.SetActive(true);
            leftChooseSlot.gameObject.SetActive(false);
            centorChooseSlot.gameObject.SetActive(false);
            rightChooseSlot.gameObject.SetActive(false);
        }
        else//小怪战
        {
            string[] layers = {
        "0",
        "1",
        "2",
    };
            ArrayUtil.Shuffle(layers);
            int number = layerCount / 2;
            string mapName_1 = $"Layer_{number + 1}_{layers[0]}";
            leftChooseSlot.Init(mapName_1);
            leftChoose.SetBtnEvent(() =>
            {
                OnHide();
                //注册目标的id
                //切换到下一场
                if (BattleManager.instance)
                    BattleManager.instance.ClearBattleField();
                nextMapName = mapName_1;
                GameManager.instance.EnterRogueBattle(1);
            });
            string mapName_2 = $"Layer_{number + 1}_{layers[1]}";
            Debug.LogError(mapName_2);
            centorChooseSlot.Init(mapName_2);
            centorChoose.SetBtnEvent(() =>
            {
                OnHide();
                //注册目标的id
                //切换到下一场
                if (BattleManager.instance)
                    BattleManager.instance.ClearBattleField();
                nextMapName = mapName_2;
                GameManager.instance.EnterRogueBattle(1);
            });
            string mapName_3 = $"Layer_{number + 1}_{layers[2]}";
            Debug.LogError(mapName_3);
            rightChooseSlot.Init(mapName_3);
            rightChoose.SetBtnEvent(() =>
            {
                OnHide();
                //注册目标的id
                //切换到下一场
                if (BattleManager.instance)
                    BattleManager.instance.ClearBattleField();
                nextMapName = mapName_3;
                GameManager.instance.EnterRogueBattle(1);
            });
            bossChooseSlot.gameObject.SetActive(false);
            leftChooseSlot.gameObject.SetActive(true);
            centorChooseSlot.gameObject.SetActive(true);
            rightChooseSlot.gameObject.SetActive(true);
        }
    }
}

