using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EmotionBubbleManager : Singleton<EmotionBubbleManager>
{
    //EmotionBubbleManager 通过读取固定文件中的Sprite图片将图片与其名字组成字典
    Dictionary<string, Sprite> emotionBubbleSpriteDic = new Dictionary<string, Sprite>();
    //Dictionary<Transform, GameObject> emotionBubbleDic = new Dictionary<Transform, GameObject>();


    public string prefabName = "EmotionBubble";
    protected override void Init()
    {
        //1.获取所有Sprite
        var loadedSprites = Resources.LoadAll<Sprite>("EmotionBubble/");
        for (int i = 0; i < loadedSprites.Length; i++)
        {
            //2.生成对应关系

            emotionBubbleSpriteDic[loadedSprites[i].name] = loadedSprites[i];
        }
        //3.写入文件
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }


    public GameObject ShowEmotionBubble(Transform target, string spriteName)
    {
        //从对象池生成GB给GB的TransFrom赋Parent
        //在GB中生成
        //在GB上获取EmotionBubbleUI给Image赋予spriteName的Sprite
        GameObject gameObject = GameObjectPoolManager.Instance.Spawn(DataBaseManager.Instance.configMap[prefabName], DialogManager.instance.canvasParent);
        gameObject.transform.localEulerAngles = Vector3.zero;
        gameObject.GetComponent<EmotionBubbleUI>().image.sprite = emotionBubbleSpriteDic[spriteName];
        gameObject.GetComponent<EmotionBubbleUI>().target = target;
        gameObject.SetActive(true);
        return gameObject;

    }


}
