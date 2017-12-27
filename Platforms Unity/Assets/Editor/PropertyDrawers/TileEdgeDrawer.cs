using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TileEdge))]
public class TileEdgeDrawer : PropertyDrawer{

    private SerializedProperty oneX, oneZ, twoX, twoZ;
    private string name;
    //private bool cache;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        //if (!cache) { // uncommented because it seems to interfere with properties shown in the dictionary
        name = property.displayName;

        property.Next(true);
        //SerializedProperty one = property.Copy();
        property.Next(true);
        oneX = property.Copy();
        property.Next(true);
        oneZ = property.Copy();

        property.Next(true);
        property.Next(true);
        //SerializedProperty two = property.Copy();
        twoX = property.Copy();
        property.Next(true);
        twoZ = property.Copy();
        property.Next(true);
        //cache = true;
        //}

        Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

        //show the X and Y from the point
        EditorGUIUtility.labelWidth = 14f;
        contentPosition.width *= 0.2f;
        //EditorGUI.indentLevel = 1;

        // one:
        EditorGUI.BeginProperty(contentPosition, label, oneX);
        {
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.IntField(contentPosition, new GUIContent("X"), oneX.intValue);
            if (EditorGUI.EndChangeCheck())
                oneX.intValue = newVal;
        }
        EditorGUI.EndProperty();

        contentPosition.x += contentPosition.width;

        EditorGUI.BeginProperty(contentPosition, label, oneZ);
        {
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.IntField(contentPosition, new GUIContent("Z"), oneZ.intValue);
            if (EditorGUI.EndChangeCheck())
                oneZ.intValue = newVal;
        }

        contentPosition.x += contentPosition.width;

        // two:
        EditorGUI.BeginProperty(contentPosition, label, twoX);
        {
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.IntField(contentPosition, new GUIContent("X"), twoX.intValue);
            if (EditorGUI.EndChangeCheck())
                twoX.intValue = newVal;
        }

        contentPosition.x += contentPosition.width;

        EditorGUI.BeginProperty(contentPosition, label, twoZ);
        {
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.IntField(contentPosition, new GUIContent("Z"), twoZ.intValue);
            if (EditorGUI.EndChangeCheck())
                twoZ.intValue = newVal;
        }
        //EditorGUI.indentLevel = 0;
        EditorGUI.EndProperty();
    }
}
