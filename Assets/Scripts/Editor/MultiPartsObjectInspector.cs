using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiPartsObject))]
public class MultiPartsObjectInspector : UnityEditor.Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var obj = (MultiPartsObject)target;

        if (GUILayout.Button("Setup"))
            obj.Setup();

        GUILayout.BeginVertical();
        DisplayNodeInfo(obj.Root);
        GUILayout.EndVertical();
    }

    public void DisplayNodeInfo(Node node)
    {
        if (node != null)
        {
            EditorGUILayout.LabelField(node.GameObject.name, EditorStyles.helpBox);
            foreach (var child in node.Childs) {
                EditorGUI.indentLevel++;
                DisplayNodeInfo(child);
                EditorGUI.indentLevel--;
            }
        }
    }
}
