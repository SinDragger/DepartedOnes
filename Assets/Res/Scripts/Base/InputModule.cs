using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入模块
/// 激活后
/// </summary>
public class InputModule : IInputModule
{
    public static Dictionary<string, InputModule> keyValues = new Dictionary<string, InputModule>();
    public bool tempBlock;
    bool isActive;
    public Dictionary<string, InputResponseNode> keyDic = new Dictionary<string, InputResponseNode>();
    public Dictionary<string, InputResponseNode> keyDownDic = new Dictionary<string, InputResponseNode>();
    public Dictionary<string, InputResponseNode> keyUpDic = new Dictionary<string, InputResponseNode>();
    public Dictionary<MouseCode, InputResponseNode> mouseDic = new Dictionary<MouseCode, InputResponseNode>();

    public virtual void ClearDic()
    {
        keyDic.Clear();
        keyDownDic.Clear();
        keyUpDic.Clear();
        mouseDic.Clear();
    }

    //TODO 增加对取消事件的监听。例如场景切换加载

    public virtual void Init()
    {
        keyValues.Add(this.GetType().Name, this);
    }

    public void RegistInputModuleToSet(string key)
    {
        InputManager.Instance.RegistInputModuleToSet(key, this);
    }

    public void RegistQuickKeyDown(object code, Action result)
    {
        keyDownDic[code.ToString()] = new InputResponseNode((o) => { result?.Invoke(); return true; }, 0);
    }

    protected bool InputToDoNothing(object param)
    {
        return true;
    }
    protected bool BlockInput(object param)
    {
        return false;
    }
    
    public virtual void Active()
    {
        
        if (isActive) return;
        
        isActive = true;
        foreach (var node in keyDic)
        {
            InputManager.Instance.keyResponseChain.RegistInputResponse(node.Key, node.Value);
        }
        foreach (var node in keyDownDic)
        {
            InputManager.Instance.keyDownResponseChain.RegistInputResponse(node.Key, node.Value);
        }
        foreach (var node in keyUpDic)
        {
            InputManager.Instance.keyUpResponseChain.RegistInputResponse(node.Key, node.Value);
        }
        foreach (var node in mouseDic)
        {
            InputManager.Instance.mouseResponseChain.RegistInputResponse(node.Key, node.Value);
        }
    }

    public virtual void Deactive()
    {
        if (!isActive) return;
        isActive = false;
        foreach (var node in keyDic)
        {
            InputManager.Instance.keyResponseChain.RemoveInputResponse(node.Key, node.Value);
        }
        foreach (var node in keyDownDic)
        {
            InputManager.Instance.keyDownResponseChain.RemoveInputResponse(node.Key, node.Value);
        }
        foreach (var node in keyUpDic)
        {
            InputManager.Instance.keyUpResponseChain.RemoveInputResponse(node.Key, node.Value);
        }
        foreach (var node in mouseDic)
        {
            InputManager.Instance.mouseResponseChain.RemoveInputResponse(node.Key, node.Value);
        }
    }

    public void ActiveBlock()
    {
        if (isActive)
        {
            Debug.Log((this.GetType().Name));
            tempBlock = true;
            Deactive();
        }
    }

    public void ActiveResume()
    {
        if (!isActive && tempBlock)
        {
            tempBlock = false;
            Active();
        }

    }

    public void ChangeMouseNodePriority(MouseCode mouseCode, int priority)
    {
        mouseDic[mouseCode].priority = priority;
        InputManager.Instance.mouseResponseChain.SortNode(mouseCode);
    }

    public void ChangeKeyNodePriority(string keyName, int priority)
    {
        keyDic[keyName].priority = priority;
        InputManager.Instance.keyResponseChain.SortNode(keyName);
    }

    public void ChangeKeyDownNodePriority(string keyName, int priority)
    {
        keyDownDic[keyName].priority = priority;
        InputManager.Instance.keyDownResponseChain.SortNode(keyName);
    }

    public void ChangeKeyUpNodePriority(string keyName, int priority)
    {
        keyUpDic[keyName].priority = priority;
        InputManager.Instance.keyUpResponseChain.SortNode(keyName);
    }

    public void ChangeDicPriority(int priority)
    {
        foreach (var node in keyDic)
        {
            node.Value.priority = priority;
        }
        foreach (var node in keyDownDic)
        {
            node.Value.priority = priority;
        }
        foreach (var node in keyUpDic)
        {
            node.Value.priority = priority;
        }
        foreach (var node in mouseDic)
        {
            node.Value.priority = priority;
        }
    }


}

public interface IInputModule
{
    void Active();
    void Deactive();
    void Init();
}
