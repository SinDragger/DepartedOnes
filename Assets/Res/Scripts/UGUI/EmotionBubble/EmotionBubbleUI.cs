using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionBubbleUI : MonoBehaviour
{
    //功能
    //1.给Image赋值Sprite-不在这里做
    //2.LateUpdate跟随Target
    //3.启动携程 控制gb回收
    public Image image;

    public Transform target;
    public float yOffset = 75;
    private void LateUpdate()
    {
        Vector3 focusPosition = Camera.main.WorldToScreenPoint(target.position) - new Vector3(Screen.width / 2, Screen.height / 2 - yOffset, 0);
        focusPosition.z = 0;
        gameObject.transform.localPosition = focusPosition;

    }

    private void OnEnable()
    {
        StartCoroutine(ReturnToPool());
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        GameObjectPoolManager.Instance.Recycle(gameObject, DataBaseManager.Instance.configMap["EmotionBubble"]);

    }
   
}
