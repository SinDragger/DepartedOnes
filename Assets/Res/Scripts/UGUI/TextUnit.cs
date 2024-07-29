using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TextUnit : Text
{
    [SerializeField]
    public List<Sprite> spriteInfoList;
    public List<Image> subImages;
    public List<int> extendSize;
    /// <summary>
    /// 拓展大小
    /// </summary>
    const float bigger = 2f;
    public void SpriteClear()
    {
        if (spriteInfoList == null) spriteInfoList = new List<Sprite>();
        spriteInfoList.Clear();
        for (int i = 0; i < subImages.Count; i++)
        {
            subImages[i].gameObject.SetActive(false);
        }
    }

    public void SetSprite(int flag, Sprite sprite)
    {
        if (spriteInfoList == null) spriteInfoList = new List<Sprite>();
        for (int i = spriteInfoList.Count; i <= flag; i++)
        {
            spriteInfoList.Add(null);
        }
        spriteInfoList[flag] = sprite;
        for (int i = subImages.Count; i < spriteInfoList.Count; i++)
        {
            var newObject = new GameObject();
            newObject.transform.parent = transform;
            //newObject.transform.localPosition = Vector3.zero;
            newObject.transform.localScale = Vector3.one;
            newObject.transform.localEulerAngles = Vector3.zero;
            subImages.Add(newObject.AddComponent<Image>());
        }
        if (subImages != null && subImages.Count > flag)
        {
            subImages[flag].sprite = sprite;
            subImages[flag].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// TODO:IDName图片语法构筑与注入
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string outText)
    {
        //m_Text = text;
        text = outText;
        RefreshSubImages();
        //<img=Iterrange>
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        if (spriteInfoList == null)
        {
            return;
        }
        var count = this.spriteInfoList.Count;

        if (count > 0)
        {
            RefreshSubImages();
            var vertexLastIndex = toFill.currentVertCount - 1;
            var spriteLastIndex = count - 1;
            var leftBottom = new UIVertex();
            for (int i = spriteLastIndex; i > -1; --i)
            {
                var index = vertexLastIndex - (spriteLastIndex - i) * 4;
                toFill.PopulateUIVertex(ref leftBottom, index);
                toFill.SetUIVertex(leftBottom, index - 1);
                toFill.SetUIVertex(leftBottom, index - 2);
                toFill.SetUIVertex(leftBottom, index - 3);
                //subImages[i].SetNativeSize();
                var imageRT = subImages[i].rectTransform;
                var pos = (Vector2)leftBottom.position;
                //增加偏移值
                pos.x += (float)fontSize / (float)50 + imageRT.sizeDelta.x / 2;
                pos.y += (float)fontSize / 3;// - imageRT.sizeDelta.y / 2;
                imageRT.localPosition = pos;
            }
        }
    }
    private void RefreshSubImages()
    {
        int startFlag = 0;
        int endFlag = 0;
        //字体相关的占位大小
        int fontSizeRelate = fontSize / 3 * 2;
        if (fontSizeRelate <= 0) return;
        //subImages.RemoveAll((image) => image == null);
        for (int i = subImages.Count; i > spriteInfoList.Count; i--)
        {
            Destroy(subImages[i - 1].gameObject);
            subImages.RemoveAt(i - 1);
        }
        StringBuilder reSizeBuild = new StringBuilder(m_Text);
        //Start,End，置换文本
        Dictionary<Image, (int, int, string)> insertDic = new Dictionary<Image, (int, int, string)>();
        for (int i = 0; i < subImages.Count; i++)
        {
            //1:分割Quad区域-确定与Image的相关
            int extend = extendSize.Count > i ? extendSize[i] : 0;
            Vector2 rectSize = subImages[i].rectTransform.sizeDelta;
            float x = subImages[i].rectTransform.sizeDelta.x;
            float width = (float)subImages[i].sprite.texture.width / (float)subImages[i].sprite.texture.height;
            rectSize.y = x * (float)subImages[i].sprite.texture.height / (float)subImages[i].sprite.texture.width;
            rectSize = rectSize / x * (fontSizeRelate + extend) * width * bigger;

            startFlag = m_Text.IndexOf("<quad", startFlag);
            if (startFlag == -1) break;
            endFlag = m_Text.IndexOf(">", startFlag);
            int size = fontSizeRelate + extend + fontSize / 50;
            size = (int)(size * bigger);
            subImages[i].rectTransform.sizeDelta = rectSize;
            insertDic[subImages[i]] = (startFlag, endFlag, $"<quad material=1 width={string.Format("{0:f2}", width)} size={size}>");
            //2:确定基础的width 以及依托width的Size
            startFlag = endFlag;
        }
        //3:从后向前进行切除与分割（避免重构）
        for (int i = subImages.Count - 1; i >= 0; i--)
        {
            if (insertDic.ContainsKey(subImages[i]))
            {
                reSizeBuild.Remove(insertDic[subImages[i]].Item1, insertDic[subImages[i]].Item2 - insertDic[subImages[i]].Item1 + 1);
                reSizeBuild.Insert(insertDic[subImages[i]].Item1, insertDic[subImages[i]].Item3);
            }
        }
        m_Text = reSizeBuild.ToString();
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        RefreshSubImages();
    }
#endif
}