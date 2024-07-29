using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DialogManager : MonoSingleton<DialogManager>
{

    public string dialogBubbleName;

    //字典 transfrom和prefab的
    public Dictionary<GameObject, Transform> bubbleDic = new Dictionary<GameObject, Transform>();
    // public GameObject dialogBubblePrefab;
    public Transform canvasParent;

    float yOffset = 75f;
    [SerializeField]
    List<string> bubbleSentence;


    // Update is called once per frame

    public void UpdateShowBubble()
    {

        foreach (var bubble in bubbleDic)
        {
            //根据对应的transfrom坐标获取物体在屏幕中的屏幕坐标 并与屏幕的中心点做差值

            Vector3 focusPosition = Camera.main.WorldToScreenPoint(bubble.Value.position) - new Vector3(Screen.width / 2, Screen.height / 2 - yOffset, 0);
            focusPosition.z = 0;
            bubble.Key.transform.localPosition = focusPosition;


        }
    }
    
    public void SetBubbleTragetTransFrom(Transform target)
    {
        //从气泡语音集合中随机一个传给bubble
        SetBubbleTragetTransFromAndSentence(target,bubbleSentence[Random.Range(0,bubbleSentence.Count-1)]);


    }

    public void SetBubbleTragetTransFromAndSentence(Transform target, string characterSentence)
    {
        //TODO 对象池生成
        if (bubbleDic.ContainsValue(target))
        {
            return;
        }
        GameObject gameObject = GameObjectPoolManager.Instance.Spawn(DataBaseManager.Instance.configMap[dialogBubbleName], canvasParent);
        gameObject.transform.localEulerAngles = Vector3.zero;
        bubbleDic[gameObject] = target;
        // gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

        gameObject.GetComponentInChildren<TextUnit>().SetText(characterSentence);
        gameObject.SetActive(true);

        //将这个携程的启动交给上一级管理器

        StartCoroutine(ClosePrefabCoroutine(gameObject));
    }

    IEnumerator ClosePrefabCoroutine(GameObject gameObject)
    {

        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        GameObjectPoolManager.Instance.Recycle(gameObject, DataBaseManager.Instance.configMap[dialogBubbleName]);
        bubbleDic.Remove(gameObject);

    }

}
