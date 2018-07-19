/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiPartsObject))]
public class MultiPartsObjectInspector : UnityEditor.Editor {
    MultiPartsObject obj;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        obj = (MultiPartsObject)target;

        if (GUILayout.Button("Setup"))
            obj.Setup();

        if (GUILayout.Button("Fit"))
            obj.FitToScale(obj.Root,obj.VirtualScale);

        if (GUILayout.Button("Reset"))
            obj.ResetTransform(obj.Root);

        GUILayout.Space(10);
        GUILayout.Label("Tree");
        GUILayout.BeginVertical();
        DisplayNodeInfo(obj.Root);
        GUILayout.EndVertical();
    }

    public void DisplayNodeInfo(Node node)
    {
        if (node != null)
        {
            //node.Selected = GUILayout.Toggle(node.Selected,node.GameObject.name, "Button");
            bool selected = GUILayout.Toggle(node.Selected, node.GameObject.name, "Button");
            if (selected)
                obj.Select(node.GameObject);
            else
                obj.Deselect(node.GameObject);

            foreach (var child in node.Childs) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                DisplayNodeInfo(child);
                GUILayout.EndHorizontal();
            }
        }
    }
}
