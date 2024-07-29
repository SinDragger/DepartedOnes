using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///// <summary>
///// 第一步 开放接口 使得有多种方式进入剧情模式
///// 第二步 解析对话文本，剧情脚本，音效，逻辑Logic
///// 第三步 根据玩家操作 显示文本，播放音效，执行逻辑
///// 第四步 对话文本-完成 
///// 第五步 执行逻辑- 未完成
///// 第六步 播放音效- 未完成
///// </summary>
/// <summary>
/// 启动剧情模式方法，传入NPC GameObject以及剧情脚本ID、启动剧情模式InputModule 
/// 通过剧情脚本ID在DataBaseManager中找到对应的剧情脚本 将其交给StoryScriptPlayer
/// ScriptPlayer将剧情脚本读取到最后，调用退出剧情模式方法， 关闭InputModule
/// 
/// </summary>
public class StoryManager : MonoSingleton<StoryManager>
{
    //故事模式输入模式

    public StoryScript nowScript;
    Dictionary<string, StoryScript> storyStore = new Dictionary<string, StoryScript>();
    public StoryScriptPlayer storyPlayer = new StoryScriptPlayer();

    //关闭其他输入模式 根据剧情脚本打开对应的输入模式
    // public InputModule inputModule = new InputModule();
    void Start()
    {
        Init();
    }
    public void Init()
    {
        //inputModule 
        GameManager.timeRelyMethods += OnUpdate;
        // StartCoroutine(StartC());
    }
    void OnUpdate(float deltaTime)
    {
        storyPlayer.ProcessStoryAciton();
    }
    
    public void ActiveStoryMode(string storyID, params object[] additionalParams)
    {
        //TODO GameObject 的Character还没做
        Active();
        StartPlaying(storyID);

    }
    public void Exit()
    {
        nowScript = null;
        Deactive();
    }
    public void SetNowStoryFlag(string flag)
    {
        //依据Key进行寻找
    }

    public void CompleteStoryMode()
    {
        //NPC角色怎么办
    }
    public void StartPlaying(string StoryTxtName)
    {
        //安全性考虑？
        storyStore[StoryTxtName] = new StoryScript(DataBaseManager.Instance.LoadStoryTest(StoryTxtName));
        nowScript = storyStore[StoryTxtName];
        storyPlayer.StartPlaying(nowScript);
    }


    public void Active()
    {
        InputManager.Instance.TempBlockInputModuleSet("Normal");
        InputManager.Instance.TempBlockInputModuleSet("Battle");

    }
    public void Deactive()
    {
        InputManager.Instance.TempBlockInputModuleSet("Dialogue");
        InputManager.Instance.ResumeInputModuleSet("Normal");
        InputManager.Instance.ResumeInputModuleSet("Battle");
    }
    //TODO 显示EMOJI
    //TODO 关闭EMOJI
    public Character GetCharacter(string idName)
    {
        if (nowScript == null) return null;

        (string, string, string) result = nowScript.castMemberList.Find((c) => c.Item1 == idName);
        if (result.Item1 == null)
        {
            return null;
        }


        return CharacterManager.Instance.GetCharacter(result);

    }

    //public void ExitStory(string[] paramLine)
    //{
    //    DialogManager.instance.boxUI.gameObject.SetActive(false);
    //    Deactive();
    //}

    /// <summary>
    /// 根据脚本名称解析脚本转化为DialogData[]数组
    /// </summary>
    /// <param name="StoryTxtName"></param>





}

public class StoryScript
{

    [SerializeField]
    public List<string[]> scriptDatas;
    public Func<bool> comfirmBlockEnd;
    int nextStoryData = 0;
    // string,character 
    /// <summary>
    /// 跳转字典
    /// </summary>
    public Dictionary<string, int> transferDic;
    /// <summary>
    /// 演职人员表
    /// </summary>
    public List<(string, string, string)> castMemberList=new List<(string, string, string)>();
    public bool isEnd => nextStoryData >= scriptDatas.Count;
    public StoryScript(List<string> scriptDatas)
    {
        Debug.Log(scriptDatas.Count);
        PreloadStory(scriptDatas);
    }

    public void Reset()
    {
        nextStoryData = -1;
        scriptDatas = null;
    }

    public string[] UpdateStory()
    {
        if (nextStoryData < scriptDatas.Count)
        {
            var data = scriptDatas[nextStoryData];
            nextStoryData++;
            return data;
        }
        return null;
    }
    /// <summary>
    /// 预加载：确定剧情脚本所需资源
    /// </summary>
    public void PreloadStory(List<string> scriptDatas)
    {
        this.scriptDatas = new List<string[]>();
        for (int i = 0; i < scriptDatas.Count; i++)
        {
            var vs = scriptDatas[i].Split(' ');
            this.scriptDatas.Add(vs);

            //TODO:确定跳转符-填入跳转字典 

            //确定所需要异步加载的资源总量   怎么做 我不知道5555
        }
    }




}