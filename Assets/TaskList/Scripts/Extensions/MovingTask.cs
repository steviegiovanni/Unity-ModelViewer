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

        public MovingTask(GameObject go, Vector3 position) :base(go)
        {
            Position = position;
        }

        public MovingTask(SerializableTask task) : base(task)
        {
            Position = task.Position;
            SnapThreshold = task.SnapThreshold;
        }

        public override void CheckTask()
        {
            if (!IsCurrentTask()) return;
            Debug.Log("Moving check task");
            Finished = Vector3.Distance(GameObject.transform.position, Position) <= SnapThreshold;
        }
    }
}
