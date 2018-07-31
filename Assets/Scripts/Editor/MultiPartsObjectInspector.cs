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
                obj.ResetAll(obj.Root);

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
				if (GUILayout.Button (node.GameObject.name + (node.Selected?"*":""), "Button")) {
					obj.ToggleSelect(node);
				}
                if (GUILayout.Button((node.Locked ? "X" : " "), "Button",GUILayout.Width(20)))
                {
                    if (node.Locked)
                        node.Locked = false;
                    else
                    {
                        obj.Release();
                        obj.Deselect(node);
                        node.Locked = true;
                    }
                        
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
