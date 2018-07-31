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
            {
                obj.ResetAll(obj.Root);
                obj.transform.localScale = Vector3.one;
            }

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
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
				if (GUILayout.Button (node.Name + " ("+(node.GameObject==null?"Missing!": node.GameObject.name)+")" + (node.Selected?"*":""), "Button")) {
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

                if (GUILayout.Button("Add Task", "Button", GUILayout.Width(100)))
                {
                    TaskList tl = obj.TaskList;
                    tl.Tasks.Add(new Task(node.GameObject,node.P0));
                }
                GUILayout.EndHorizontal();

                if (node.Selected)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.Width(100));
                    node.Name = GUILayout.TextField(node.Name);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
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
