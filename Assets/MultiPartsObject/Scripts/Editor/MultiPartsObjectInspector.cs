﻿/// author: Stevie Giovanni

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
            // draw default inspector gui
            base.OnInspectorGUI();

            obj = (MultiPartsObject)target;

            // to setup internal data structure after inserting the gameobject
            if (GUILayout.Button("Setup"))
                obj.Setup();

            // to fit the object into the cage size
            if (GUILayout.Button("Fit"))
                obj.FitToScale(obj.Root, obj.VirtualScale);

            // to reset all transform position, rotation, and scale
            if (GUILayout.Button("Reset"))
            {
                obj.ResetAll(obj.Root);
                obj.transform.localScale = Vector3.one;
            }

            GUILayout.Space(10);

            // draw the tree structure
            GUILayout.Label("Tree");
            indentLevel = 0;
            DisplayNodeInfo(obj.Root);
        }

        int indentLevel;

        /// <summary>
        /// displaying each node info
        /// </summary>
        public void DisplayNodeInfo(Node node)
        {
            if (node != null)
            {
				// node name and go, lock button, reset button, add task button
                GUILayout.BeginHorizontal();
                GUILayout.Space(indentLevel * 10);
				if (GUILayout.Button (node.Name + " ("+(node.GameObject==null?"Missing!": node.GameObject.name)+")" + (node.Selected?"*":""), "Button")) {
                    EditorGUIUtility.PingObject(node.GameObject);
                    obj.ToggleSelect(node);
				}

                // to lock or unlock node for interaction
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

                // to reset this specific node
                if (GUILayout.Button("R", "Button", GUILayout.Width(20)))
                {
                    obj.Reset(node);
                }

                // to add a task related to the game object of this node
                if (GUILayout.Button("Add Task", "Button", GUILayout.Width(100)))
                {
                    GenericMenu genericMenu = new GenericMenu();
                    for (int i = 0; i < MultiPartsObjectEditorUtility.TaskTypes().Length; i++)
                    {
                        genericMenu.AddItem(new GUIContent(MultiPartsObjectEditorUtility.TaskTypes()[i]), false,
                            (param) =>
                            {
                                int index = (int)param;
                                switch (index)
                                {
                                    case 0:
                                        {
                                            TaskList tl = obj.TaskList;
                                            tl.Tasks.Add(new MovingTask(node.GameObject, obj.transform.TransformPoint(node.P0), node.R0));
                                        }
                                        break;
                                    case 1:
                                        {
                                            TaskList tl = obj.TaskList;
                                            tl.Tasks.Add(new GenericTask(node.GameObject));
                                        }
                                        break;
                                }
                            }
                        , i);
                    }
                    genericMenu.ShowAsContext();
                }
                GUILayout.EndHorizontal();


                // node details
                if (node.Selected)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(indentLevel * 10);
                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    // allows to change the name of the node
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.Width(100));
                    node.Name = GUILayout.TextField(node.Name);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }

                // node childs
                if (node.Childs.Count > 0)
                {
                    indentLevel++;
                    GUILayout.BeginVertical();
                    foreach (var child in node.Childs)
                    {
                        //GUILayout.BeginHorizontal();
                        //GUILayout.Space(20);
                        DisplayNodeInfo(child);
                        //GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                    indentLevel--;
                }
            }
        }
    }
}
