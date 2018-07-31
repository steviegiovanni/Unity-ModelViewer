using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModelViewer
{
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


        public override void OnInspectorGUI()
        {
            obj = (TaskList)target;
            GUILayout.BeginVertical();
            for(int i=0; i < obj.Tasks.Count; i++) {
                GUILayout.BeginHorizontal();

                string taskName = "Missing!";
                if(obj.Tasks[i].GameObject != null) {
                    Node node = null;
                    if (obj.MPO.Dict.TryGetValue(obj.Tasks[i].GameObject,out node)){
                        taskName = node.Name + " ("+node.GameObject.name+")";
                    }
                }

                bool clicked = GUILayout.Toggle(i == SelectedTaskId, taskName,EditorStyles.toolbarButton);
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
                GUILayout.EndHorizontal();

                if(i == SelectedTaskId)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("TEST");
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
        }
    }
}
