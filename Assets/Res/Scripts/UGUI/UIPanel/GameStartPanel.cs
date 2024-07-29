using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartPanel : UIPanel
{
    public override string uiPanelName => "GameStartPanel";
    public Button startButton;
    public Button challengeButton;
    public Button settingButton;
    public Button exitButton;
    public Toggle difficultCheck;

    public GameObject tempGameObject;
    public GameObject storyPanel;

    public Text text;
    public Button nextClick;

    public override void OnInit()
    {
        CheckDifficultMode();
        startButton.SetBtnEvent(() =>
        {
            StartCoroutine(PlayStory(() =>
            {
                tempGameObject.SetActive(false);
                OnHide();
                GameManager.instance.StartGame();
            }));
        });
        challengeButton.SetBtnEvent(() =>
        {
            OnHide();
            GameManager.instance.EnterChallengeMode();
        });
        settingButton.SetBtnEvent(() =>
        {
            OnHide();
            GameManager.instance.StartGame();
        });
        exitButton.SetBtnEvent(() =>
        {
            //OnHide();
            GameManager.instance.EndGame();
        });
        difficultCheck.onValueChanged.AddListener((value) =>
        {
            DataBaseManager.Instance.SetTempData<bool>("UseDiffcultMode", value);
        });
        base.OnInit();
    }

    public override void OnShow(bool withAnim = true)
    {
        CheckDifficultMode();
        base.OnShow(withAnim);
    }

    void CheckDifficultMode()
    {
        bool isDifficult = DataBaseManager.Instance.GetTempData<bool>("UseDiffcultMode");
        difficultCheck.SetIsOnWithoutNotify(isDifficult);
    }

    bool onClick;

    string[] story ={
    $@"在太过漫长的黑暗时代之中，魂灵们的痛苦回响不息

渴求着能终结战争与苦难的救世者降临",
    $@"无数埃铎斯子民遗愿、祈望构成了强大的源初奇迹仪式

铸造超脱生与死，永不被击垮的存在",
$@"以骸骨亡灵形态复苏，灵魂彻底蜕变熊熊燃烧

为众人而死的战争英雄归来了",
$@"一位不死不灭的守护者

这个时代的不义希望",
    };

    IEnumerator PlayStory(System.Action callback)
    {
        callback?.Invoke();
        yield break;
        storyPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        for (int i = 0; i < story.Length; i++)
        {
            text.text = story[i];
            text.DOFade(1f, 0.4f);
            yield return new WaitForSecondsRealtime(0.4f);
            onClick = false;
            while (!onClick)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    onClick = true;
                }
                yield return 0;
            }
            text.DOFade(0f, 0.4f);
            yield return new WaitForSecondsRealtime(0.4f);
        }
        callback?.Invoke();
    }
}
