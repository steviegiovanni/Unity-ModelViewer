/// author: Stevie Giovanni 

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// custom editor for TaskList showing all the tasks
    /// </summary>
    [CustomEditor(typeof(TaskList))]
    public class TaskListInspector : UnityEditor.Editor
    {
        TaskList obj;

        private int _selectedTaskId = -1;
        public int SelectedTaskId
        {
            get { return _selectedTaskId; }
            set
            {
                if(_selectedTaskId != value)
                {
                    _selectedTaskId = value;
                    EditorGUI.FocusTextInControl(string.Empty);
                }
            }
        }

        /// <summary>
        /// display a particular task detail. case by case depending on the task type
        /// </summary>
        public void DisplayTaskDetail(Task t) 
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            switch (t.GetType().Name)
            {
                case "MovingTask":
                    {
                        MovingTask castedt = (MovingTask)t;

                        // to modify snap threshold
                        GUILayout.Label("Snap Threshold");
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        castedt.SnapThreshold = GUILayout.HorizontalSlider(castedt.SnapThreshold, 0.1f, 1f);
                        GUILayout.Label(castedt.SnapThreshold.ToString(),GUILayout.Width(30));
                        GUILayout.EndHorizontal();

                        // to modify goal position
                        GUILayout.Label("Goal");
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label("x: ");
                        float x = EditorGUILayout.FloatField(castedt.Position.x);
                        GUILayout.Label("y: ");
                        float y = EditorGUILayout.FloatField(castedt.Position.y);
                        GUILayout.Label("z: ");
                        float z = EditorGUILayout.FloatField(castedt.Position.z);
                        castedt.Position = new Vector3(x, y, z);
                        GUILayout.Space(20);
                        GUILayout.EndHorizontal();

                        // easily get the go current position as the goal pos
                        if (GUILayout.Button("Use Current GO Position"))
                        {
                            if (castedt.GameObject != null)
                                castedt.Position = castedt.GameObject.transform.position;
                        }
                    }
                    break;
            }
            GUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            // draw base inspector gui
            base.OnInspectorGUI();

            obj = (TaskList)target;
            GUILayout.BeginVertical();
            for(int i=0; i < obj.Tasks.Count; i++) {
                GUILayout.BeginHorizontal();

                // toggle show task detail on and off
                string taskName = "Missing!";
                if(obj.Tasks[i].GameObject != null) {
                    taskName = " (" + obj.Tasks[i].GameObject.name + ")";
                    /*Node node = null;
                    if (obj.MPO.Dict.TryGetValue(obj.Tasks[i].GameObject,out node)){
                        taskName = node.Name + " ("+node.GameObject.name+")";
                    }*/
                }

                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                style.alignment = TextAnchor.MiddleLeft;
                bool clicked = GUILayout.Toggle(i == SelectedTaskId, i+1 + ". " + taskName,style);
                if (clicked != (i == SelectedTaskId))
                {
                    if (clicked)
                    {
                        SelectedTaskId = i;
                        GUI.FocusControl(null);
                    }
                    else
                    {
                        SelectedTaskId = -1;
                    }
                }

                // to easily reorder tasks up and down
                if (i <= 0)
                    GUI.enabled = false;
                if (GUILayout.Button(i>0?"^":" ", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    Task temp = obj.Tasks[i];
                    obj.Tasks[i] = obj.Tasks[i - 1];
                    obj.Tasks[i - 1] = temp;
                }
                GUI.enabled = true;

                // to easily reorder tasks up and down
                if (i >= obj.Tasks.Count - 1)
                    GUI.enabled = false;
                if (GUILayout.Button(i<obj.Tasks.Count - 1?"v":" ", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    Task temp = obj.Tasks[i];
                    obj.Tasks[i] = obj.Tasks[i + 1];
                    obj.Tasks[i + 1] = temp;
                }
                GUI.enabled = true;

                // remove task from task list
                if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    if (SelectedTaskId != -1)
                        SelectedTaskId = -1;
                    obj.Tasks.RemoveAt(i);
                }

                GUILayout.EndHorizontal();

                // display task detail if task is selected
                if(i == SelectedTaskId)
                {
                    DisplayTaskDetail(obj.Tasks[i]);
                }
            }
            GUILayout.EndVertical();
        }
    }
}
