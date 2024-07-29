using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//优化成不依赖mono的单例
public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image hintUI;
    [SerializeField] private Image uiMask;
    public float maskFocusSpeed;

    private TutorialButton tutorialButton;
    private Text hintText;

    private Coroutine currentTutorial;
    private Coroutine uimaskFocus;
    private bool waitButton;

    private void Awake()
    {
        tutorialButton = GameObjectPoolManager.Instance.Spawn("Prefab/TutorialButton").GetComponent<TutorialButton>();
        tutorialButton.transform.SetParent(canvas.transform);
        hintText = hintUI.GetComponentInChildren<Text>();
        
        tutorialButton.gameObject.SetActive(false);

        hintUI.gameObject.SetActive(false);
        uiMask.gameObject.SetActive(false);
    }


    public void StartTutorial(string tutorialId, bool inputDisable)
    {
        InputManager.Instance.inputingText = inputDisable;
    }

    [Button]
    public void StartBasicTutorial()
    {
        currentTutorial = StartCoroutine(Tutorial_BasicControl());


    }

    // 放 飞 自 我
    public IEnumerator Tutorial_BasicControl()
    {
        InputManager.Instance.inputingText = true;
        //todo更新，复数ui会爆炸
        LegionMarkUI playerLegionUI = LegionManager.Instance.GetUI("TestPlayerLegion");
        CameraControl.Instance.LookAtV3(playerLegionUI.transform.position);

        //应该是判断距离，但是现在能用
        while(CameraControl.Instance.transform.parent.position != playerLegionUI.transform.position)
            yield return null;

        tutorialButton.OnLeftClick.AddListener(() => { playerLegionUI.OnUIClick(); ResetTutorialButton(); });
        waitButton = true;
        tutorialButton.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, playerLegionUI.transform.position);
        //button加上偏移量?
        tutorialButton.gameObject.SetActive(true);
        uimaskFocus = StartCoroutine(ScreenMask(tutorialButton.transform.position));
        hintText.text = "点击选中军队";
        hintUI.gameObject.SetActive(true);

        while(waitButton)
            yield return null;
        hintUI.gameObject.SetActive(false);

        SectorConstructionUI constructionUI = ConstructionManager.Instance.GetMapUI("伐木场");

        CameraControl.Instance.LookAtV3(constructionUI.transform.position);
        while (CameraControl.Instance.transform.parent.position != constructionUI.transform.position)
            yield return null;

        waitButton = true;
        tutorialButton.OnRightClick.AddListener(() =>
        {
            int constructionBelong = constructionUI.sectorConstruction.belong;
            if (constructionBelong == -1 || constructionBelong == playerLegionUI.legion.belong && constructionUI.sectorConstruction.stationedLegions.Count == 0)
            {
                playerLegionUI.legion.State = new LegionMoveState(playerLegionUI.legion, constructionUI.mapPos, (distance) =>
                {
                    if (distance == 0)
                    {
                        playerLegionUI.legion.State = new LegionOccupyState(playerLegionUI.legion, constructionUI.sectorConstruction);
                        if (UIManager.Instance.IsSelected(playerLegionUI))
                        {
                            EventManager.Instance.DispatchEvent(new EventData(MapEventType.CONSTRUCTION_SELECT, "Building", constructionUI));
                            UIManager.Instance.SetNowSelect(constructionUI, playerLegionUI);
                        }
                    }
                }, Color.gray);
                ResetTutorialButton();
            }
        });
        tutorialButton.gameObject.SetActive(true);
        uimaskFocus = StartCoroutine(ScreenMask(tutorialButton.transform.position));
        hintText.text = "右键移动至" + constructionUI.sectorConstruction.constructionName;
        hintUI.gameObject.SetActive(true);

        while (waitButton)
            yield return null;

        hintText.text = "按住left alt可加速时间";

        while (Vector3.Distance(playerLegionUI.transform.position,constructionUI.transform.position) > 0.05f)
            yield return null;


        Debug.Log("目前教程结束");

        hintUI.gameObject.SetActive(false);
        CameraControl.Instance.ChangeCameraToBattle();
        InputManager.Instance.inputingText = false;
    }

    private void ResetTutorialButton()
    {
        waitButton = false;
        tutorialButton.OnLeftClick.RemoveAllListeners();
        tutorialButton.OnRightClick.RemoveAllListeners();
        tutorialButton.gameObject.SetActive(false);
        uiMask.gameObject.SetActive(false);
    }


    private IEnumerator ScreenMask(Vector3 screenPos)
    { 
        uiMask.gameObject.SetActive(true);
        uiMask.material.SetFloat("_CircleX", screenPos.x);
        uiMask.material.SetFloat("_CircleY", screenPos.y);
        uiMask.material.SetFloat("_CircleRadius", Screen.height);

        float radius = Screen.height;
        float targetRadius = tutorialButton.GetComponent<Image>().mainTexture.width / 2;
        float currentRadius = radius;
        while (currentRadius > targetRadius)
        {
            currentRadius -= (radius - targetRadius) * Time.deltaTime * maskFocusSpeed;
            uiMask.material.SetFloat("_CircleRadius", currentRadius);
            yield return null;
        }
        uimaskFocus = null;
    }

}
