using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    public Button selfButton;
    public Image iconImage;
    public Action clickCallback;
    public Text titleText;
    public Text desText;

    public Image selectBorder;
    private void Awake()
    {
        selfButton.onClick.AddListener(() =>
        {
            clickCallback?.Invoke();
        });
    }
    public void Init(string titleType, string subType)
    {
        if (titleText)
            titleText.text = titleType;
        if (desText)
            desText.text = subType;
    }
    public void OnSelected(bool value)
    {
        selectBorder.gameObject.SetActive(value);
    }
}
