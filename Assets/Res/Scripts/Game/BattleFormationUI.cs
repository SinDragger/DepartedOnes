using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFormationUI : MonoBehaviour
{
    public GameObject border;
    public int flag;
    public Text showText;
    public bool isShow;

    public GameObject prefab;
    List<GameObject> nowAbleList = new List<GameObject>();
    int lastCommands = 0;
    private void OnEnable()
    {
        TimeManager.Instance.RegistRealTimeCycleAction(0.3f, UpdateCountNumber);
    }

    private void OnDisable()
    {
        TimeManager.Instance.RemoveRealTimeCycleAction(UpdateCountNumber);
    }

    void UpdateCountNumber()
    {
        int count = UnitControlManager.instance.GetFormationAbleCommanderAliveNum(flag);
        int commands = UnitControlManager.instance.GetFormationAbleCommanderNum(flag);
        if (count > 0 && !isShow)
        {
            OnShow();
        }
        if (count == 0 && isShow)
        {
            OnHide();
        }
        if (commands != lastCommands)
        {
            commands = lastCommands;
            RefreshCommandIcon();
        }
        showText.text = count.ToString();
    }

    void RefreshCommandIcon()
    {
        var sprites = UnitControlManager.instance.GetFormationUIs(flag);
        sprites.Sort((a, b) => a.GetHashCode().CompareTo(b.GetHashCode()));
        for (int i = nowAbleList.Count; i < sprites.Count; i++)
        {
            nowAbleList.Add(Instantiate(prefab, prefab.transform.parent));
        }
        for (int i = sprites.Count; i < nowAbleList.Count; i++)
        {
            nowAbleList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            nowAbleList[i].SetActive(true);
            //设置localPosition
            nowAbleList[i].GetComponent<Image>().sprite = sprites[i];
        }
        float min = -25f;
        float max = 25f;
        switch (sprites.Count)
        {
            case 2:
                min = -12.5f;
                max = 12.5f;
                break;
        }
        if (sprites.Count > 1)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                nowAbleList[i].transform.localPosition = new Vector3(0f + Mathf.Lerp(min, max, (float)i / (float)(sprites.Count - 1)), 0f, 0f);
            }
        }
        else
        {
            nowAbleList[0].transform.localPosition = Vector3.zero;
        }
    }

    void OnShow()
    {
        isShow = true;
        border.transform.DOLocalMove(Vector3.zero, 0.2f);
    }
    void OnHide()
    {
        isShow = false;
        border.transform.DOLocalMove(new Vector3(-85f, 0f, 0f), 0.2f);
    }
}
