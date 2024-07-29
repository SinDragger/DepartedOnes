using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prologue : MonoBehaviour
{
    //创建NPC 黑屏幕 组件显示屏幕

    void Start()
    {
        StartCoroutine(StartStoryCoroutine());
    }
    IEnumerator StartStoryCoroutine() {
        yield return null;
        StoryManager.instance.ActiveStoryMode("StoryTest");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
