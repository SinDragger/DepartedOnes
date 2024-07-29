using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TextUnit))]
[CanEditMultipleObjects]
public class TextUnitEditor : ProxyEditor
{
    private SerializedProperty spriteInfoList;
    protected override void OnEnable()
    {
        base.OnEnable();
        spriteInfoList = serializedObject.FindProperty("spriteInfoList");
    }
    protected override void GetEditorTypeName(out string typeFullName, out string dllName)
    {
        typeFullName = "UnityEditor.UI.TextEditor";
        dllName = "UnityEditor.UI";
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(spriteInfoList);
        serializedObject.ApplyModifiedProperties();
    }
}
