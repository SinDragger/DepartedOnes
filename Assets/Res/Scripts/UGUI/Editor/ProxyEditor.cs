using System;

public abstract class ProxyEditor : UnityEditor.Editor
{
    private Type _editorType;
    private UnityEditor.Editor _editorInstance;

    protected abstract void GetEditorTypeName(out string typeFullName, out string dllName);

    protected virtual void OnEnable()
    {
        if (null != _editorInstance)
            return;

        if (null == _editorType)
        {
            //var typeFullName = "UnityEditor.UI.TextEditor";
            //var dllName = "UnityEditor.UI";
            GetEditorTypeName(out var typeFullName, out var dllName);
            if (string.IsNullOrEmpty(dllName))
                _editorType = Type.GetType(typeFullName);
            else
                _editorType = Type.GetType($"{typeFullName},{dllName}");
        }
        if (null == _editorType) return;

        if (null != targets)
            _editorInstance = CreateEditor(targets, _editorType);
    }

    private void OnDisable()
    {
        if (null != _editorInstance)
        {
            DestroyImmediate(_editorInstance);
            _editorInstance = null;
        }
    }

    public override void OnInspectorGUI()
    {
        if (null != _editorInstance)
            _editorInstance.OnInspectorGUI();
    }

}