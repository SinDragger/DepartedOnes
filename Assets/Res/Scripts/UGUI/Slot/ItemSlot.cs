using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Text rewardText;
    public Image iconImage;
    public Image backImage;
    [SerializeField]
    protected GameObject CompleteObj;
    public GameObject[] effects;
    public ExtraAttach[] extraAttachs;

    public bool SetText(string text)
    {
        if (rewardText == null) return false;
        rewardText.text = text;
        return true;
    }
    public bool SetBackImage(Sprite sprite)
    {
        if (backImage == null) return false;
        backImage.sprite = sprite;
        return true;
    }

    public bool SetImage(Sprite sprite, bool useOrigin = false)
    {
        if (iconImage == null) return false;
        if (iconImage.sprite != sprite)
            iconImage.sprite = sprite;
        if (useOrigin)
        {
            iconImage.SetNativeSize();
        }
        return true;
    }

    public void SetComplete(bool isComplete)
    {
        if (CompleteObj)
        {
            CompleteObj.SetActive(isComplete);
        }
    }

    public static void SetImageSize(Image img)
    {
        Vector2 sizeBefore = img.transform.GetComponent<RectTransform>().sizeDelta;

        img.SetNativeSize();

        Vector2 size = img.transform.GetComponent<RectTransform>().sizeDelta;

        bool isX = true;

        float l;

        if (size.x > size.y)
        {
            l = size.x;
        }
        else
        {
            isX = false;

            l = size.y;
        }

        float scale = (isX ? sizeBefore.x : sizeBefore.y) / l;

        img.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x * scale, size.y * scale);
    }

    public void SetEffect(bool flag)
    {
        if (effects != null)
        {
            foreach (var e in effects)
            {
                e.SetActive(flag);
            }
        }
    }
    public void FadeAndDestorySelf(float time)
    {
        var graphics = GetComponentsInChildren<Graphic>(true);
        if (graphics != null)
            foreach (var graphic in graphics)
            {
                graphic.DOFade(0f, time);
            }
        CoroutineManager.StartDelayedCoroutine(time, () => Destroy(gameObject));
    }

    public void HideSlot()
    {
        if (rewardText) rewardText.gameObject.SetActive(false);
        if (iconImage) iconImage.gameObject.SetActive(false);
    }
    public void ShowSlot()
    {
        if (rewardText) rewardText.gameObject.SetActive(true);
        if (iconImage) iconImage.gameObject.SetActive(true);
    }
    /// <summary>
    /// 播放Slot的动画
    /// </summary>
    /// <param name="callback"></param>
    public void PlayEffectAnim(string keyFlag,string targetUIName, System.Action callback = null)
    {
        callback?.Invoke();
    }


    public bool TryGetUI(out GameObject target, string targetName)
    {
        target = null;
        foreach (var g in extraAttachs)
        {
            if (g != null && g.name == targetName && g.targetUI != null)
            {
                target = g.targetUI;
                return true;
            }
        }
        return false;
    }
    public bool TryGetUI<T>(out T target, string targetName) where T : MonoBehaviour
    {
        target = null;
        foreach (var g in extraAttachs)
        {
            if (g != null && g.name == targetName && g.targetUI != null)
            {
                target = g.targetUI.GetComponent<T>();
                return target != null;
            }
        }
        return false;
    }
}

[System.Serializable]
public class ExtraAttach
{
    public string name;
    public GameObject targetUI;
}