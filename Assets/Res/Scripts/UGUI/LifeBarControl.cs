using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBarControl : MonoBehaviour
{
    public GameObject thickLinePrefab;
    public GameObject thinLinePrefab;
    List<GameObject> thickList = new List<GameObject>();
    List<GameObject> thinList = new List<GameObject>();
    public GameObject left;
    public GameObject right;

    const float unitLength = 40f;

    public void Init(int maxNumber)
    {
        //float nowAble = maxNumber / unitLength;
        float delta = 1f / (float)maxNumber * unitLength;
        float nowDelta = 0f;
        int flag = 0;
        while (nowDelta + delta < 1f)
        {
            nowDelta += delta;
            Vector3 targetPos = left.transform.localPosition + (right.transform.localPosition - left.transform.localPosition) * nowDelta;
            if (thinList.Count <= flag)
            {
                thinList.Add(Instantiate(thinLinePrefab, transform));
                thinList[flag].gameObject.SetActive(true);
            }
            thinList[flag].transform.localPosition = targetPos;
            flag++;
        }
        for (int i = flag; i < thinList.Count; i++)
        {
            thinList[i].gameObject.SetActive(false);
        }
    }
}
