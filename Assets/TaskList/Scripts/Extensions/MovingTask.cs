/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    public enum MovingTaskType
    {
        MoveTo,
        AwayFrom
    }


    /// <summary>
    /// represents a task that requires the user to bring an object to a specific position
    /// </summary>
    public class MovingTask : Task
    {
        /// <summary>
        /// the end position to which the object must be brought to
        /// </summary>
        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// the end rotation of the object
        /// </summary>
        private Quaternion _rotation;
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// the threshold in which the object will snap to the position
        /// </summary>
        private float _snapThreshold = 0.1f;
        public float SnapThreshold
        {
            get { return _snapThreshold; }
            set { _snapThreshold = value; }
        }

        /// <summary>
        /// whether we have to move to or away from a certain location
        /// </summary>
        private MovingTaskType _moveType;
        public MovingTaskType MoveType
        {
            get { return _moveType; }
            set { _moveType = value; }
        }

        /// <summary>
        /// constructor taking a go and position
        /// </summary>
        public MovingTask(GameObject go, Vector3 position, Quaternion rotation) :base(go)
        {
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public MovingTask(SerializableTask task) : base(task)
        {
            Position = task.Position;
            Rotation = task.Rotation;
            SnapThreshold = task.SnapThreshold;
            MoveType = task.MoveType;
        }

        /// <summary>
        /// reimplementation of checktask. simply checks whether the position of the go is close to the goal position
        /// </summary>
        public override void CheckTask()
        {
            Finished = (MoveType==MovingTaskType.MoveTo)?(Vector3.Distance(GameObject.transform.position, Position) <= SnapThreshold): (Vector3.Distance(GameObject.transform.position, Position) > SnapThreshold);
            if (Finished)
            {
                // snap to position if movetype is  moveto
                if (MoveType == MovingTaskType.MoveTo)
                {
                    GameObject.transform.position = Position;
                    GameObject.transform.rotation = Rotation;
                }
            }
        }

        /// <summary>
        /// reimplemntation of draw task hint. draw the silhouette of the game object at the goal pos
        /// </summary>
        public override void DrawTaskHint(TaskList taskList)
        {
            taskList.Hint = GameObject.Instantiate(GameObject,Position,GameObject.transform.rotation);
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
            if(GameObject != null && GameObject.GetComponent<MeshFilter>() != null)
            {
                Color shadowColor = Color.magenta;
                Gizmos.color = shadowColor;
                Gizmos.DrawMesh(GameObject.GetComponent<MeshFilter>().sharedMesh,Position,GameObject.transform.rotation,GameObject.transform.lossyScale);
                Color selectedColor = Color.yellow;
                Gizmos.color = selectedColor;
                Gizmos.DrawMesh(GameObject.GetComponent<MeshFilter>().sharedMesh, GameObject.transform.position, GameObject.transform.rotation, GameObject.transform.lossyScale);
            }
        }
    }
}
