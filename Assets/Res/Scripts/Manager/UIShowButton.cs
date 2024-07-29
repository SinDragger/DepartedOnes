using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIShowButton : MonoBehaviour
{
    public Button button;
    public string uiFormName;
    public string uiGroupName;
    // Start is called before the first frame update
    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        if (button != null)
        {
            button.SetBtnEvent(() =>
            {
                UIManager.Instance.ShowUI(uiFormName, groupName: uiGroupName);
            });
        }
    }
}
