using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoSingleton_AutoCreate<InputManager>
{
    public bool isShiftMulAble;
    public InteractiveArea mouseInteractiveArea;
    InteractiveArea nowInteractiveArea;

    public Vector3 mouseWorldPos;
    public Collider nowMouseOnWorldCollider;
    Vector2 leftDeltaV;
    public Vector2 leftFirstV;

    Vector2 _leftPrevPosition;

    Vector2 middleDeltaV;
    Vector2 middleFirstV;
    Vector2 _middlePrevPosition;

    public Vector2 rightFirstV;
    Vector2 rightDeltaV;
    Vector2 _rightPrevPosition;
    /// <summary>
    /// 主游戏界面的输入
    /// </summary>
    public Dictionary<KeyCode, string> battleSceneInputTrans = new Dictionary<KeyCode, string>();

    Dictionary<string, HashSet<InputModule>> inputModuleSet = new Dictionary<string, HashSet<InputModule>>();
    public InputPointType inputMode { get; private set; }
    /// <summary>
    /// 持续按键的
    /// </summary>
    public InputResponseChain<string> keyResponseChain = new InputResponseChain<string>();
    /// <summary>
    /// 短期按键Down
    /// </summary>
    public InputResponseChain<string> keyDownResponseChain = new InputResponseChain<string>();
    /// <summary>
    /// 短期按键Up
    /// </summary>
    public InputResponseChain<string> keyUpResponseChain = new InputResponseChain<string>();
    /// <summary>
    /// 鼠标交互
    /// </summary>
    public InputResponseChain<MouseCode> mouseResponseChain = new InputResponseChain<MouseCode>();

    public bool inputingText;
    protected override void Init()
    {
        if (hasInit) return;
        hasInit = true;
        SwitchToDefaultKeySet();
    }

    private void Start()
    {
        Init();
    }
    //防止update正在遍历中改变字典 引发报错
    public bool needSwitchToDefault;
    public bool needSwitchToARPG;
    public void SwitchToDefaultKeySet()
    {
        battleSceneInputTrans.Clear();

        battleSceneInputTrans[KeyCode.A] = InputType.CAMERA_LEFT.ToString();
        battleSceneInputTrans[KeyCode.W] = InputType.CAMERA_UP.ToString();
        battleSceneInputTrans[KeyCode.D] = InputType.CAMERA_RIGHT.ToString();
        battleSceneInputTrans[KeyCode.S] = InputType.CAMERA_DOWN.ToString();
        battleSceneInputTrans[KeyCode.Q] = InputType.CAMERA_ROTATE_LEFT.ToString();
        battleSceneInputTrans[KeyCode.E] = InputType.CAMERA_ROTATE_RIGHT.ToString();
        battleSceneInputTrans[KeyCode.KeypadMinus] = InputType.TIME_MINUS.ToString();
        battleSceneInputTrans[KeyCode.KeypadPlus] = InputType.TIME_ADD.ToString();
        battleSceneInputTrans[KeyCode.KeypadMultiply] = InputType.TIME_MAX.ToString();
        battleSceneInputTrans[KeyCode.KeypadDivide] = InputType.TIME_MIN.ToString();
        //battleSceneInputTrans[KeyCode.Space] = InputType.TIME_SWITCH.ToString();
        battleSceneInputTrans[KeyCode.Tab] = InputType.TAB.ToString();
        //编组
        battleSceneInputTrans[KeyCode.Alpha1] = InputType.Formation1.ToString();
        battleSceneInputTrans[KeyCode.Alpha2] = InputType.Formation2.ToString();
        battleSceneInputTrans[KeyCode.Alpha3] = InputType.Formation3.ToString();
        battleSceneInputTrans[KeyCode.Alpha4] = InputType.Formation4.ToString();

        battleSceneInputTrans[KeyCode.F2] = InputType.SELECT_ALL_ABLE_TROOP.ToString();

        // currentInputTrans = battleSceneInputTrans;
    }
    public void SwitchToARPGKeySet()
    {
        battleSceneInputTrans.Clear();
        //角色移动
        battleSceneInputTrans[KeyCode.W] = InputType.PLAYER_UP.ToString();
        battleSceneInputTrans[KeyCode.A] = InputType.PLYAER_LEFT.ToString();
        battleSceneInputTrans[KeyCode.S] = InputType.PLAYER_DOWN.ToString();
        battleSceneInputTrans[KeyCode.D] = InputType.PLAYER_RIGHT.ToString();
        //角色技能
        battleSceneInputTrans[KeyCode.Space] = InputType.PLAYER_SKILL2.ToString();
        battleSceneInputTrans[KeyCode.LeftShift] = InputType.PLAYER_SKILL1.ToString();
        battleSceneInputTrans[KeyCode.R] = InputType.PLAYER_SKILL3.ToString();
        battleSceneInputTrans[KeyCode.Alpha4] = InputType.PLAYER_ULTIMATE.ToString();
        //切换模式
        battleSceneInputTrans[KeyCode.Tab] = InputType.TAB.ToString();

        //currentInputTrans = battleSceneARPGInputTrans;
    }
    /// <summary>
    /// 当激活剧情时 需要激活对应的按键
    /// </summary>
    public void DialogKeySet()
    {
        battleSceneInputTrans[KeyCode.Space] = InputType.Skip.ToString();
#if UNITY_EDITOR
        battleSceneInputTrans[KeyCode.Escape] = InputType.ESC.ToString();
#endif
    }

    public void RegistInputModuleToSet(string key, InputModule inputModule)
    {
        if (!inputModuleSet.ContainsKey(key)) inputModuleSet[key] = new HashSet<InputModule>();
        inputModuleSet[key].Add(inputModule);
    }

    public void TempBlockInputModuleSet(string key)
    {

        foreach (InputModule inputModule in inputModuleSet[key])
        {

            inputModule.ActiveBlock();
        }
    }
    public void ResumeInputModuleSet(string key)
    {
        foreach (var inputModule in inputModuleSet[key])
        {
            inputModule.ActiveResume();
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (needSwitchToARPG)
        {
            needSwitchToARPG = false;
            SwitchToARPGKeySet();
        }

        if (needSwitchToDefault)
        {
            needSwitchToDefault = false;
            SwitchToDefaultKeySet();
        }
        UpdateRayCast();
        RefreshInput();
        if (inputingText) return;
        isShiftMulAble = Input.GetKey(KeyCode.LeftShift);
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        HandleTouchInput();
#else
        HandleMouseInput();
#endif
        //滚轮
        float delta = Input.GetAxis("Mouse ScrollWheel");
        foreach (var item in mouseResponseChain.keyResponseDic)
        {
            switch (item.Key)
            {
                case MouseCode.SCROLL:
                    if (item.Key == 0)
                    {
                        if (item.Value != null && item.Value.Count > 0)
                        {
                            //责任链询问
                            if (delta != 0f)
                            {
                                mouseResponseChain.Trigger(item.Key, delta);
                            }
                        }
                    }
                    break;
            }
        }

        foreach (var pair in battleSceneInputTrans)
        {

            if (Input.GetKey(pair.Key))
            {
                if (keyResponseChain.keyResponseDic.ContainsKey(pair.Value))
                {
                    keyResponseChain.Trigger(pair.Value);
                }
            }
            if (Input.GetKeyDown(pair.Key))
            {
                if (keyDownResponseChain.keyResponseDic.ContainsKey(pair.Value))
                {
                    keyDownResponseChain.Trigger(pair.Value);
                }
            }
            if (Input.GetKeyUp(pair.Key))
            {
                if (keyUpResponseChain.keyResponseDic.ContainsKey(pair.Value))
                {
                    keyUpResponseChain.Trigger(pair.Value);
                }
            }
        }
    }

    List<RaycastResult> raycastResults;
    public void SetInputMode(InputPointType inputPointType)
    {
        inputMode = inputPointType;
    }

    float maxMove;
    private void HandleMouseInput()
    {
        //增加当前交互内容的列表
        if (mouseInteractiveArea != null || nowInteractiveArea != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                nowInteractiveArea = mouseInteractiveArea;
                leftFirstV = Input.mousePosition;
                maxMove = float.MinValue;
            }
            if (nowInteractiveArea != null && Input.GetMouseButton(0))
            {
                Vector2 delta = (Vector2)Input.mousePosition - leftFirstV;
                float distance = Vector2.Distance(delta, Vector2.zero);
                if (distance > maxMove) maxMove = distance;
            }

            if (nowInteractiveArea != null && Input.GetMouseButtonUp(0))
            {
                Vector2 delta = (Vector2)Input.mousePosition - leftFirstV;
                float distance = Vector2.Distance(delta, Vector2.zero);
                if (mouseInteractiveArea != nowInteractiveArea && distance > 10)
                {
                    //Drag触发
                    nowInteractiveArea.TriggerOnDrag((Vector2)Input.mousePosition - leftFirstV);
                }
                else if (maxMove < 10)
                {
                    //Click触发
                    nowInteractiveArea.TriggerClick();
                }
                nowInteractiveArea = null;
            }
            return;
        }
        FomulaLeftInputHandle(Input.mousePosition, () => Input.GetMouseButtonDown(0), () => Input.GetMouseButton(0), () => Input.GetMouseButtonUp(0));
        FomulaMiddleInputHandle(Input.mousePosition, () => Input.GetMouseButtonDown(2), () => Input.GetMouseButton(2), () => Input.GetMouseButtonUp(2));
        FomulaRightInputHandle(Input.mousePosition, () => Input.GetMouseButtonDown(1), () => Input.GetMouseButton(1), () => Input.GetMouseButtonUp(1));
    }
    /// <summary>
    /// 当前鼠标左键拖拽移动距离
    /// </summary>
    public float mouseLeftDeltaDistance;
    public float mouseRightDeltaDistance;
    private void FomulaLeftInputHandle(Vector2 input, Func<bool> startFunc, Func<bool> pressFunc, Func<bool> endFunc)
    {
        if (startFunc())//鼠标按下
        {
            leftDeltaV = Vector2.zero;
            _leftPrevPosition = input;
            leftFirstV = _leftPrevPosition;
            inputMode = ReadNowInputMode();//可能是UI
            mouseLeftDeltaDistance = 0f;
            mouseResponseChain.Trigger(MouseCode.LEFT_DOWN, input);
        }

        else if (pressFunc())//鼠标按住
        {
            Vector2 curPosition = input;
            //一定范围内不做处理
            leftDeltaV = _leftPrevPosition - curPosition;
            _leftPrevPosition = curPosition;
            mouseLeftDeltaDistance = Vector2.Distance(curPosition, leftFirstV);
            mouseResponseChain.Trigger(MouseCode.LEFT, input);
        }
        else if (endFunc())//鼠标抬起
        {
            RefreshInputOnUI();
            mouseResponseChain.Trigger(MouseCode.LEFT_UP, input);
            inputMode = InputPointType.NONE;
        }
    }
    private void FomulaMiddleInputHandle(Vector2 input, Func<bool> startFunc, Func<bool> pressFunc, Func<bool> endFunc)
    {
        if (startFunc())//鼠标按下
        {
            middleDeltaV = Vector2.zero;
            _middlePrevPosition = input;
            middleFirstV = _middlePrevPosition;
            mouseResponseChain.Trigger(MouseCode.MIDDLE_DOWN, input);
        }
        else if (pressFunc())//鼠标按住
        {
            Vector2 curPosition = input;
            middleDeltaV = _middlePrevPosition - curPosition;
            _middlePrevPosition = curPosition;
            mouseResponseChain.Trigger(MouseCode.MIDDLE, input);
        }
        else if (endFunc())//鼠标抬起
        {
            mouseResponseChain.Trigger(MouseCode.MIDDLE_UP, input);
            inputMode = InputPointType.NONE;
        }
    }

    /// <summary>
    /// 右键输入
    /// </summary>
    private void FomulaRightInputHandle(Vector2 input, Func<bool> startFunc, Func<bool> pressFunc, Func<bool> endFunc)
    {
        if (startFunc())//鼠标按下
        {
            rightDeltaV = Vector2.zero;
            _rightPrevPosition = input;
            rightFirstV = _rightPrevPosition;
            inputMode = ReadNowInputMode();//可能是UI
            mouseRightDeltaDistance = 0f;
            mouseResponseChain.Trigger(MouseCode.RIGHT_DOWN, input);
        }
        else if (pressFunc())//鼠标按住
        {
            Vector2 curPosition = input;
            rightDeltaV = _rightPrevPosition - curPosition;
            _rightPrevPosition = curPosition;
            mouseRightDeltaDistance = Vector2.Distance(curPosition, rightFirstV);
            mouseResponseChain.Trigger(MouseCode.RIGHT, input);
        }
        else if (endFunc())//鼠标抬起
        {
            mouseResponseChain.Trigger(MouseCode.RIGHT_UP, input);
            inputMode = InputPointType.NONE;
        }
    }
    public void SimulateMouseRight(Vector2 pos, float time, Action callback)
    {
        StartCoroutine(SimulateMouseRightCor(pos, time, callback));
    }
    public void SimulateMouseRight(Vector2 pos)
    {
        Vector2 curPosition = pos;
        rightDeltaV = _rightPrevPosition - curPosition;
        _rightPrevPosition = curPosition;
        mouseRightDeltaDistance = Vector2.Distance(curPosition, rightFirstV);
        mouseResponseChain.Trigger(MouseCode.RIGHT, pos);
    }

    IEnumerator SimulateMouseRightCor(Vector2 pos, float time, Action callback)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            SimulateMouseRight(pos);
            yield return 0;
        }
        callback?.Invoke();
    }

    public void SimulateMouseRightDown(Vector2 pos)
    {
        rightDeltaV = Vector2.zero;
        _rightPrevPosition = pos;
        rightFirstV = _rightPrevPosition;
        inputMode = ReadNowInputMode();//可能是UI
        mouseRightDeltaDistance = 0f;
        mouseResponseChain.Trigger(MouseCode.RIGHT_DOWN, pos);
    }
    public void SimulateMouseRightUp(Vector2 pos)
    {
        //815 424
        mouseResponseChain.Trigger(MouseCode.RIGHT_UP, pos);
        inputMode = InputPointType.NONE;
    }
    /// <summary>
    /// 读取当前指令
    /// </summary>
    InputPointType ReadNowInputMode()
    {
        if (RefreshInputOnUI())
        {
            return InputPointType.UI;
        }
        return InputPointType.NONE;
    }
    void RefreshInput()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
    }
    /// <summary>
    /// 刷新在场景内的点击
    /// </summary>
    public bool RefreshInputOnUI()
    {
        if (raycastResults.Count == 0)
        {
            return false;
        }
        else
        {
            foreach (var result in raycastResults)
            {
                if (result.gameObject.layer > 4)//TODO:不能用标准值
                {
                    return true;
                }
            }
            return false;
        }
    }
    RaycastHit m_HitInfo = new RaycastHit();
    public void UpdateRayCast()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = 65;// LayerMask.GetMask("Default","Map");//MiniGame
        if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo, float.MaxValue, MapMask))//,1000f,c
        {
            if (m_HitInfo.collider == null)
            {
                mouseWorldPos = Vector3.zero;
                nowMouseOnWorldCollider = null;
            }
            else
            {
                //Debug.LogError(m_HitInfo.collider.name);
                mouseWorldPos = m_HitInfo.point;
                nowMouseOnWorldCollider = m_HitInfo.collider;
            }
        }
    }

    public const int MapMask = 65;

    public bool IsNowRaycastMapLayer()
    {
        if (raycastResults != null && raycastResults.Count > 0)
        {
            if (raycastResults[0].gameObject.layer == 6) return true;
        }
        return false;
    }

    /// <summary>
    /// 依据当前点击寻找Component
    /// </summary>
    public T NowRaycastFind<T>(int maxParentCheck = 2, int maxRaycastLayer = 1)
    {
        if (maxRaycastLayer > raycastResults.Count) maxRaycastLayer = raycastResults.Count;
        for (int i = 0; i < maxRaycastLayer; i++)
        {
            Transform t = raycastResults[i].gameObject.transform;
            T result = default;
            for (int j = 0; j < maxParentCheck; j++)
            {
                if (t == null) break;
                result = t.GetComponent<T>();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    t = t.parent;
                }
            }
        }
        return default;
    }

    public RaycastResult NowRaycastSearch(Func<RaycastResult,bool> checkFunc)
    {
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (checkFunc.Invoke(raycastResults[i]))
            {
                return raycastResults[i];
            }
        }
        return default;
    }

    //public bool TriggerSceenRayCast(Action<Vector3> method)
    //{
    //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
    //    {
    //        if (m_HitInfo.collider == null) return false;
    //        method?.Invoke(m_HitInfo.point);
    //        return true;
    //    }
    //    return false;
    //}

}

public enum InputType
{
    //基础
    ESC,
    //相机相关
    CAMERA_UP,
    CAMERA_DOWN,
    CAMERA_RIGHT,
    CAMERA_LEFT,
    CAMERA_SCROLL,
    CAMERA_ROTATE_LEFT,
    CAMERA_ROTATE_RIGHT,
    //时间相关
    TIME_MIN,
    TIME_MAX,
    TIME_SWITCH,
    TIME_ADD,
    TIME_MINUS,
    //玩家移动
    PLAYER_UP,
    PLAYER_DOWN,
    PLAYER_RIGHT,
    PLYAER_LEFT,
    //玩家释放技能
    PLAYER_SKILL1,
    PLAYER_SKILL2,
    PLAYER_SKILL3,
    PLAYER_ULTIMATE,
    //切换
    TAB,
    //对话系统
    Skip,
    //战场
    SELECT_ALL_ABLE_TROOP,
    Formation1,
    Formation2,
    Formation3,
    Formation4,

}

public enum InputPointType
{
    NONE,
    MAP_MOVE,
    TARGET_CHOOSE,
    TROOP_MOVE,
    UI,
}


public class InputResponseChain<T>
{
    public Dictionary<T, List<InputResponseNode>> keyResponseDic = new Dictionary<T, List<InputResponseNode>>();

    public void Trigger(T target, object param = null)
    {
        if (!keyResponseDic.ContainsKey(target)) return;
        var list = keyResponseDic[target];
        for (int i = 0; i < list.Count; i++)
        {
            //list[i].trigger(param)返回false 就穿透了向下继续执行
            if (list[i].IsResponseType(InputManager.Instance.inputMode) && list[i].trigger(param))
            {
                return;
            }
        }
    }

    public void RegistInputResponse(T targetKeyCode, InputResponseNode node)
    {
        if (!keyResponseDic.ContainsKey(targetKeyCode))
        {
            keyResponseDic[targetKeyCode] = new List<InputResponseNode>();
        }
        keyResponseDic[targetKeyCode].Add(node);
        keyResponseDic[targetKeyCode].Sort();
    }
    public void RegistInputResponse(T targetKeyCode, int priority, Func<object, bool> onClick)
    {
        if (!keyResponseDic.ContainsKey(targetKeyCode))
        {
            keyResponseDic[targetKeyCode] = new List<InputResponseNode>();
        }
        keyResponseDic[targetKeyCode].Add(new InputResponseNode(onClick, priority));
        keyResponseDic[targetKeyCode].Sort();
    }

    public bool RemoveInputResponse(T targetKeyCode, Func<object, bool> onclick)
    {
        if (keyResponseDic.ContainsKey(targetKeyCode))
        {
            foreach (var node in keyResponseDic[targetKeyCode])
            {
                if (node.trigger.Equals(onclick))
                {
                    keyResponseDic[targetKeyCode].Remove(node);
                    return true;
                }
            }
        }
        return false;
    }


    public bool RemoveInputResponse(T targetKeyCode, InputResponseNode node)
    {
        if (keyResponseDic.ContainsKey(targetKeyCode))
        {
            keyResponseDic[targetKeyCode].Remove(node);
            return true;
        }
        return false;
    }

    public void SortNode(T targetKeyCode) {
        keyResponseDic[targetKeyCode].Sort();
    }
}


/// <summary>
/// 输入反馈节点
/// </summary>
public class InputResponseNode : IComparable<InputResponseNode>
{
    /// <summary>
    /// 响应优先度
    /// 例如：同样左键 有技能激活下的目标选取左键>单位选取左键
    /// </summary>
    public int priority;
    /// <summary>
    /// 响应的Type
    /// </summary>
    public InputPointType targetType;
    public Func<object, bool> trigger;
    public InputResponseNode(Func<object, bool> trigger, int priority = 0)
    {
        this.trigger = trigger;
        this.priority = priority;
    }
    public int CompareTo(InputResponseNode obj)
    {
        return obj.priority.CompareTo(priority);
    }

    public bool IsResponseType(InputPointType input)
    {
        if (targetType == InputPointType.NONE) return true;
        if (input.Equals(targetType)) return true;
        return false;
    }
}


public enum MouseCode
{
    SCROLL,
    LEFT,
    LEFT_DOWN,
    LEFT_UP,
    RIGHT,
    RIGHT_DOWN,
    RIGHT_UP,
    MIDDLE,
    MIDDLE_DOWN,
    MIDDLE_UP,
}