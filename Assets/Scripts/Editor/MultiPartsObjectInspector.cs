/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModelViewer
{
    [CustomEditor(typeof(MultiPartsObject))]
    public class MultiPartsObjectInspector : UnityEditor.Editor
    {
        MultiPartsObject obj;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            obj = (MultiPartsObject)target;

            if (GUILayout.Button("Setup"))
                obj.Setup();

            if (GUILayout.Button("Fit"))
                obj.FitToScale(obj.Root, obj.VirtualScale);

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
				GUILayout.BeginHorizontal();
				if (GUILayout.Button (node.GameObject.name, "Button")) {
					obj.ToggleSelect(node.GameObject);
				}
				if (GUILayout.Button (node.Selected ? "*" : " ", "Button",GUILayout.Width(20))) {
					obj.ToggleSelect(node.GameObject);
				}
				GUILayout.EndHorizontal();

                foreach (var child in node.Childs)
                {
                    GUILayout.BeginHorizontal();
					GUILayout.Space(20);
                    DisplayNodeInfo(child);
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
