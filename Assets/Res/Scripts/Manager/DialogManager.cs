using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 当有父级管理器时 改成Singleton
/// </summary>
public partial class DialogManager : MonoSingleton<DialogManager>
{
    //第一件事 剥夺玩家控制权 -按键输入链的切换 -完成
    //第二件事 读取对应剧情脚本 分析文件 处理文件 处理成DialogData -完成 交给剧情管理器
    //第三件事 显示对话盒子 将DialogData传入DialogControl 进行处理 -完成
    /// <summary>
    /// 第三步
    /// </summary>
    public DialogBoxUI boxUI;
    /// <summary>
    /// 对话显示完成 靠boxUI传入
    /// </summary>
    [SerializeField]
    private bool dialogueDisplayCompleted;
    //将module集成在上一级 也就是剧情控制器中StoryManager -负责按键的切换负责 给对话管理器分配任务
    public InputModule inputModule = new DialogManager_InputModule();
    private void Start()
    {
        bubbleSentence = DataBaseManager.Instance.LoadDialogBubbleTest("DialogBubble");
        Init();
    }
    public bool CompleteNowDialogue
    {
        get { return boxUI.completeNowDialogue && boxUI.isClicked; }

    }

    public void ShowDialog(string dialogueDetails)
    {
        Active();
        //启动 空格 鼠标左键 跳过对话显示阶段， 如果已经完成对话显示进入 到下一阶段 可以进行解析下一行对话内容 isClicked=true;
        boxUI.ShowDialog(dialogueDetails);

    }
    public void SetBoxName(string name)
    {
        boxUI.ShowSpeakerName(name);
    }
    public void SetBoxTransFrom(Vector3 pos)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(pos);

        //screenPoint.x -= 0.5f * Screen.width;\
        if (CameraControl.Instance.recordFOV)
        {
            //特写镜头 偏移值增加
            screenPoint.y += 150;
        }
        else
            screenPoint.y += 100;

        (boxUI.gameObject.transform as RectTransform).anchoredPosition = screenPoint;
        //boxUI.gameObject.transform.eulerAngles  = screenPoint; 
    }
    public void ActiveBox()
    {
        boxUI.gameObject.SetActive(true);
    }
    public void DeActiveBox()
    {
        boxUI.gameObject.SetActive(false);
    }
    public void SkipDialogue()
    {
        boxUI.StopAllCoroutines();
        boxUI.SkipDialog();
    }
    public void IsOnClicked()
    {
        boxUI.isClicked = true;
        inputModule.Deactive();
    }
    public void Init()
    {

        inputModule.Init();

        inputModule.RegistInputModuleToSet("Dialogue");

    }

    public void Active()
    {

        InputManager.Instance.DialogKeySet();
        inputModule.Active();

    }


    void LateUpdate()
    {

        UpdateShowBubble();
    }




}
