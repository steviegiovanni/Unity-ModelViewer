/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// represents a task
    /// </summary>
    public class Task
    {
        /// <summary>
        /// name of the task
        /// </summary>
        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }

        /// <summary>
        /// description of the task
        /// </summary>
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
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
        /// a task event is an event sequence played after user completes the task (rotate, translate a part after user clicks)
        /// </summary>
        private TaskEvent _taskEvent;
        public TaskEvent TaskEvent
        {
            get { return _taskEvent; }
            set { _taskEvent = value; }
        }

        /// <summary>
        /// check task is finished, override for different task behavior
        /// </summary>
        public virtual void CheckTask()
        {
            Finished = true;
            /*if (!IsCurrentTask()) return;
            Debug.Log("Base check task");
            if (Finished)
                TaskList.NextTask();*/
        }

        /// <summary>
        /// what kind of hint should be drawn? override for different task behaviour
        /// </summary>
        public virtual void DrawTaskHint(TaskList taskList)
        {}

        /// <summary>
        /// draw hint gizmos on editor mode
        /// </summary>
        public virtual void DrawEditorTaskHint()
        {}

        /// <summary>
        /// base constructor taking a game object
        /// </summary>
        public Task(GameObject go)
        {
            TaskName = "New Task";
            GameObject = go;
        }

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public Task(SerializableTask task)
        {
            GameObject = task.GameObject;
            TaskName = task.TaskName;
            Description = task.Description;

            SerializableTaskEvent ste = task.TaskEvent;
            if (ste != null)
            {
                switch (ste.TypeName)
                {
                    case "TransformTaskEvent":
                        {
                            TaskEvent = new TransformTaskEvent(ste);
                        }
                        break;
                }

                if(TaskEvent != null)
                    TaskEvent.Task = this;
            }
        }
    }
}
