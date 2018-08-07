/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
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
        /// the threshold in which the object will snap to the position
        /// </summary>
        private float _snapThreshold = 0.1f;
        public float SnapThreshold
        {
            get { return _snapThreshold; }
            set { _snapThreshold = value; }
        }

        /// <summary>
        /// constructor taking a go and position
        /// </summary>
        public MovingTask(GameObject go, Vector3 position) :base(go)
        {
            Position = position;
        }

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public MovingTask(SerializableTask task) : base(task)
        {
            Position = task.Position;
            SnapThreshold = task.SnapThreshold;
        }

        /// <summary>
        /// reimplementation of checktask. simply checks whether the position of the go is close to the goal position
        /// </summary>
        public override void CheckTask()
        {
            if (!IsCurrentTask()) return;
            Debug.Log("Moving check task");
            Finished = Vector3.Distance(GameObject.transform.position, Position) <= SnapThreshold;
            if (Finished)
            {
                // snap to position
                GameObject.transform.position = Position;

                MultiPartsObject mpo = TaskList.GetComponent<MultiPartsObject>();
                if (mpo != null)
                    mpo.Deselect(GameObject);

                // increment next task
                TaskList.NextTask();
            }
        }

        /// <summary>
        /// reimplemntation of draw task hint. draw the silhouette of the game object at the goal pos
        /// </summary>
        public override void DrawTaskHint()
        {
            TaskList.Hint = GameObject.Instantiate(GameObject,Position,GameObject.transform.rotation);
            if (TaskList.Hint.GetComponent<Collider>() != null)
                GameObject.Destroy(TaskList.Hint.GetComponent<Collider>());
            if (TaskList.Hint.GetComponent<Renderer>() != null)
                TaskList.Hint.GetComponent<Renderer>().material = TaskList.SilhouetteMaterial;
        }
    }
}
