using UnityEngine;
using UnityEditor;

//LootTableDrawer
[CustomPropertyDrawer(typeof(Container))]
public class CustomDropRateInspector : PropertyDrawer
{
    float dropRateProperty;
    //Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Using BeginProperty / EndProperty on the parent property means prefab override logic works on entire property
        EditorGUI.BeginProperty(position, label, property);

        //Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Dont make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        //Calculate rects
        Rect nameRect = new Rect(position.x, position.y, 60, position.height);
        Rect dropRect = new Rect(position.x + 65, position.y, 70, position.height);
        Rect dropRateRectS = new Rect(position.x + 140, position.y, 50, position.height);
        Rect dropRateRect = new Rect(position.x + 195, position.y, 30, position.height);
        Rect helperRect = new Rect(position.x + 225, position.y, 20, position.height);

        //Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(dropRect, property.FindPropertyRelative("drop"), GUIContent.none);
        EditorGUI.PropertyField(dropRateRect, property.FindPropertyRelative("dropRate"), GUIContent.none);
        EditorGUI.HelpBox(helperRect, "%", MessageType.None);

        dropRateProperty = property.FindPropertyRelative("dropRate").floatValue;
        dropRateProperty = GUI.HorizontalSlider(dropRateRectS, dropRateProperty, 0f, 100f);
        property.FindPropertyRelative("dropRate").floatValue = dropRateProperty;

        //Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(Loot))]
public class CustomLootInspector1 : PropertyDrawer
{
    float dropRateProperty;
    //Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Using BeginProperty / EndProperty on the parent property means prefab override logic works on entire property
        EditorGUI.BeginProperty(position, label, property);

        //Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Dont make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect descRect = new Rect(position.x, position.y, 60, position.height);
        Rect typeRect = new Rect(position.x + 65, position.y, 50, position.height);
        Rect goRect = new Rect(position.x + 120, position.y, 60, position.height);
        Rect matRect = new Rect(position.x + 180, position.y, 60, position.height);

        EditorGUI.PropertyField(descRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);
        EditorGUI.PropertyField(goRect, property.FindPropertyRelative("go"), GUIContent.none);
        EditorGUI.PropertyField(matRect, property.FindPropertyRelative("mat"), GUIContent.none);

        //Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }


}
