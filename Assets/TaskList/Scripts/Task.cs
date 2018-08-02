﻿/// author: Stevie Giovanni

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
        /// the owner tasklist
        /// </summary>
        private TaskList _taskList;
        public TaskList TaskList
        {
            get { return _taskList; }
            set { _taskList = value; }
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
        /// check task is finished, override for different task behavior
        /// </summary>
        public virtual void CheckTask()
        {
            if (!IsCurrentTask()) return;
            Debug.Log("Base check task");
            if (Finished)
                TaskList.NextTask();
        }

        /// <summary>
        /// what kind of hint should be drawn? override for different task behaviour
        /// </summary>
        public virtual void DrawTaskHint()
        {}

        /// <summary>
        /// base constructor taking a game object
        /// </summary>
        public Task(GameObject go)
        {
            GameObject = go;
        }

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public Task(SerializableTask task)
        {
            GameObject = task.GameObject;
        }

        /// <summary>
        /// simple check whether this task is the currently active task
        /// </summary>
        public bool IsCurrentTask()
        {
            if(TaskList != null)
            {
                return TaskList.Tasks.IndexOf(this) == TaskList.CurrentTaskId;
            }

            return false;
        }
    }
}