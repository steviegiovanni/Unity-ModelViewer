/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// represents a task that requires the user to bring an object to a specific position
    /// </summary>
    public class Task
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
        private float _snapThreshold;
        public float SnapThreshold
        {
            get { return _snapThreshold; }
            set { _snapThreshold = value; }
        }
        
        /// <summary>
        /// whether the task is finished of not
        /// </summary>
        private bool _finished = false;
        public bool Finished
        {
            get { return _finished; }
            set { _finished = value; }
        }

        /// <summary>
        /// the game object users need to interact with to complete the task
        /// </summary>
        private GameObject _go;
        public GameObject GameObject
        {
            get { return _go; }
            set { _go = value; }
        }

        /// <summary>
        /// whether this task is active or not
        /// </summary>
        private bool _active = false;
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        /// <summary>
        /// check task is finished
        /// </summary>
        public void CheckTask()
        {
            Finished = Vector3.Distance(GameObject.transform.position, Position) <= SnapThreshold;
        }

        public Task() { }

        public Task(GameObject go, Vector3 position)
        {
            GameObject = go;
            Position = position;
        }
    }

    public class TaskList : MonoBehaviour
    {
        private List<Task> _tasks;
        public List<Task> Tasks
        {
            get {
                if (_tasks == null)
                    _tasks = new List<Task>();
                return _tasks;
            }
        }

        private MultiPartsObject _mpo;
        public MultiPartsObject MPO
        {
            get {
                if (_mpo == null)
                    _mpo = GetComponent<MultiPartsObject>();
                if (_mpo == null)
                    Debug.LogError("No MPO attached");
                return _mpo;
            }
        }
    }
}
