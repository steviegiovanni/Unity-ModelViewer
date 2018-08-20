using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// represents a generic task. can be used when user selects a component and stuff need to happen
    /// </summary>
    public class ClickingTask : Task
    {
        /// <summary>
        /// constructor taking a go
        /// </summary>
        public ClickingTask(GameObject go) : base(go){}

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public ClickingTask (SerializableTask task):base(task){}

        /// <summary>
        /// reimplemntation of draw task hint. draw the silhouette of the game object at the goal pos
        /// </summary>
        public override void DrawTaskHint(TaskList taskList)
        {
            taskList.Hint = GameObject.Instantiate(GameObject, GameObject.transform.position, GameObject.transform.rotation);
            taskList.Hint.transform.localScale = GameObject.transform.lossyScale;
            if (taskList.Hint.GetComponent<Collider>() != null)
                GameObject.Destroy(taskList.Hint.GetComponent<Collider>());
            if (taskList.Hint.GetComponent<Renderer>() != null)
                taskList.Hint.GetComponent<Renderer>().material = taskList.SilhouetteMaterial;
        }

        /// <summary>
        /// draw hint gizmos on editor mode
        /// </summary>
        public override void DrawEditorTaskHint()
        {
            if (GameObject != null && GameObject.GetComponent<MeshFilter>() != null)
            {
                Color selectedColor = Color.yellow;
                Gizmos.color = selectedColor;
                Gizmos.DrawMesh(GameObject.GetComponent<MeshFilter>().sharedMesh, GameObject.transform.position, GameObject.transform.rotation, GameObject.transform.lossyScale);
            }
        }
    }
}
