using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fanchart : MonoBehaviour
{
    public GameObject origin;
    public List<GameObject> fans = new List<GameObject>();
    public Image fanSlot;


    public void Init(Dictionary<string, int> dic)
    {
        for (int i = fans.Count; i < dic.Count; i++)
        {
            fans.Add(Instantiate(origin, origin.transform.parent));
        }
        for (int i = dic.Count; i < fans.Count; i++)
        {
            fans[i].SetActive(false);
        }
        int total = 0;
        foreach(var subItem in dic) {
            total += subItem.Value;
        }
        //设定percent并进行排列
    }
}
