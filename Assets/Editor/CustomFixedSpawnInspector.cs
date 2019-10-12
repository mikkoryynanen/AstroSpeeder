using UnityEngine;
using UnityEditor;



[CustomPropertyDrawer(typeof(FixedSpawns))]
public class CustomFixedSpawnInspector : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect descRect = new Rect(position.x, position.y, 0, position.height);
        Rect sPos1 = new Rect(position.x - 50, position.y, 25, position.height);
        Rect sPos2 = new Rect(position.x - 25, position.y, 25, position.height);
        Rect sPos3 = new Rect(position.x, position.y, 25, position.height);
        Rect sPos4 = new Rect(position.x + 25, position.y, 25, position.height);
        Rect sPos5 = new Rect(position.x + 50, position.y, 25, position.height);
        Rect sPos6 = new Rect(position.x + 75, position.y, 25, position.height);
        Rect sPos7 = new Rect(position.x + 100, position.y, 25, position.height);
        Rect sPos8 = new Rect(position.x + 125, position.y, 25, position.height);
        Rect sPos9 = new Rect(position.x + 150, position.y, 25, position.height);

        EditorGUI.PropertyField(descRect, property.FindPropertyRelative("desc"), GUIContent.none);
        EditorGUI.PropertyField(sPos1, property.FindPropertyRelative("spawnPos1"), GUIContent.none);
        EditorGUI.PropertyField(sPos2, property.FindPropertyRelative("spawnPos2"), GUIContent.none);
        EditorGUI.PropertyField(sPos3, property.FindPropertyRelative("spawnPos3"), GUIContent.none);
        EditorGUI.PropertyField(sPos4, property.FindPropertyRelative("spawnPos4"), GUIContent.none);
        EditorGUI.PropertyField(sPos5, property.FindPropertyRelative("spawnPos5"), GUIContent.none);
        EditorGUI.PropertyField(sPos6, property.FindPropertyRelative("spawnPos6"), GUIContent.none);
        EditorGUI.PropertyField(sPos7, property.FindPropertyRelative("spawnPos7"), GUIContent.none);
        EditorGUI.PropertyField(sPos8, property.FindPropertyRelative("spawnPos8"), GUIContent.none);
        EditorGUI.PropertyField(sPos9, property.FindPropertyRelative("spawnPos9"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}