using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public int index = 0;
    public List<GameObject> tips = new List<GameObject>();

    public Image image;
    public Text text;
    public GameObject goLeft;
    public GameObject goRight;
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Active()
    {
        index = 0;
        gameObject.SetActive(true);
        ShowUI();
    }

    public void Next()
    {
        index++;
        ShowUI();
    }
    public void Previous()
    {
        index--;
        ShowUI();
    }

    public void ShowUI()
    {

        if (index == 0)
        {
            goLeft.SetActive(false);
        }
        else
        {
            goLeft.SetActive(true);
        }
        if (index == tips.Count - 1)
        {
            goRight.SetActive(false);
        }
        else
        {
            goRight.SetActive(true);
        }
        for (int i = 0; i < tips.Count; i++)
        {
            tips[i].SetActive(i == index);
        }
    }
}
