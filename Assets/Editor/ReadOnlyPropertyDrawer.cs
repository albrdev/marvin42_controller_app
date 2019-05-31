/// <summary>
/// Unity inspector GUI for ReadOnlyPropertyAttribute
/// <author email="albrdev@gmail.com">Alexander Brunström</author>
/// <date>2019-05-27</date>
/// </summary>

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyPropertyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
