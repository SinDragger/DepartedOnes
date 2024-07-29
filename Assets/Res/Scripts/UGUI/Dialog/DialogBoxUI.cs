using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 目前更倾向于 没有头像显示的ui 类似于八方旅人 将对话框移动到固定对话的角色身上
/// </summary>
public class DialogBoxUI : MonoBehaviour
{
    public Image leftIamge;
    public Image rightIamge;
    public TextUnit nameText;
    public TextUnit textUnit;
    public bool completeNowDialogue = false;
    public bool isClicked = false;
    string dialogueDetails;
    public void ShowSpeakerName(string name)
    {

        nameText.SetText(name);
    }

    public void ShowDialog(string dialogueDetails)
    {
        completeNowDialogue = false;
        isClicked = false;
        this.dialogueDetails = dialogueDetails;
        textUnit.SetText("");
        
        StartCoroutine(ShowDialogDetails(dialogueDetails));
    


    }

    public void SkipDialog() {
        completeNowDialogue = true;
        textUnit.SetText(dialogueDetails);
    }
    IEnumerator ShowDialogDetails(string dialogueDetails)
    {

        foreach (char letter in dialogueDetails)
        {
            textUnit.text += letter;
            //TODO 对话文字的显示速度
            yield return new WaitForSeconds(0.05f);
        }
        completeNowDialogue = true;

    }


}
