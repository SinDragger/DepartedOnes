using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public Button selfButton;
    public Image iconImage;
    public Action clickCallback;
    public Text nameText;
    public Image selectBorder;

    private void Awake()
    {
        selfButton.onClick.AddListener(() =>
        {
            clickCallback?.Invoke();
        });
    }
    public void Init(string equipName)
    {
        nameText.text = equipName;
    }
    public void OnSelected(bool value)
    {
        selectBorder.gameObject.SetActive(value);
    }
}
