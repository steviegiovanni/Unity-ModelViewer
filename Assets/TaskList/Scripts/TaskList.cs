﻿/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ModelViewer
{
    /// <summary>
    /// a tasklist represents a series of tasks associated with the loaded object model
    /// </summary>
    public class TaskList : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// the id of the currently active task
        /// </summary>
        private int _currentTaskId = 0;
        public int CurrentTaskId
        {
            get { return _currentTaskId; }
            set { _currentTaskId = value; }
        }

        /// <summary>
        /// the list of tasks
        /// </summary>
        private List<Task> _tasks;
        public List<Task> Tasks
        {
            get {
                if (_tasks == null)
                    _tasks = new List<Task>();
                return _tasks;
            }
        }

        /// <summary>
        /// the hint object, probably instantiated when each task is active
        /// </summary>
        private GameObject _hint;
        public GameObject Hint
        {
            get { return _hint; }
            set { _hint = value; }
        }

        /// <summary>
		/// silhouette material , in case needed
		/// </summary>
		[SerializeField]
        private Material _silhouetteMaterial;
        public Material SilhouetteMaterial
        {
            get { return _silhouetteMaterial; }
            set { _silhouetteMaterial = value; }
        }

        /// <summary>
        /// the serialized node structure
        /// </summary>
        [HideInInspector]
        public List<SerializableTask> serializedTasks;

        /// <summary>
        /// listeners notified when a new task starts, useful for UI
        /// </summary>
        public class TaskStartEvent : UnityEvent<Task> { }
        private TaskStartEvent _taskStartListeners;
        public TaskStartEvent TaskStartListeners
        {
            get {
                if (_taskStartListeners == null)
                    _taskStartListeners = new TaskStartEvent();
                return _taskStartListeners;
            }
        }

        /// <summary>
        /// initial position and rotation of gameobjects when the scene is started
        /// </summary>
        private List<Vector3> _goInitialPos;
        private List<Quaternion> _goInitialRot;

        /// <summary>
        /// store initial position and rotation of game objects related to the task
        /// </summary>
        public void StoreInitialPosAndRot()
        {
            _goInitialPos = new List<Vector3>();
            _goInitialRot = new List<Quaternion>();
            foreach(var task in Tasks)
            {
                _goInitialPos.Add(task.GameObject.transform.position);
                _goInitialRot.Add(task.GameObject.transform.rotation);
            }
        }

        /// <summary>
        /// reset initial position and rotation of gameobjects related to the task 
        /// </summary>
        public void ResetInitialPosAndRot()
        {
            int i = 0;
            foreach (var task in Tasks)
            {
                // store all child pos and rot cos we only want to move this specific go
                List<Vector3> childInitPos = new List<Vector3>();
                List<Quaternion> childInitRot = new List<Quaternion>();
                foreach (Transform child in task.GameObject.transform)
                {
                    childInitPos.Add(child.position);
                    childInitRot.Add(child.rotation);
                }

                // reset go pos and rotation
                task.GameObject.transform.SetPositionAndRotation(_goInitialPos[i], _goInitialRot[i]);

                // reset all child pos and rot
                int j = 0;
                foreach (Transform child in task.GameObject.transform)
                {
                    child.SetPositionAndRotation(childInitPos[j], childInitRot[j]);
                    j++;
                }

                // increment iterator
                i++;
            }
        }

        /// <summary>
        /// reset the tasklist
        /// </summary>
        public void Reset()
        {
            this.StopAllCoroutines();
            ResetInitialPosAndRot();
            foreach (var task in Tasks)
                task.Finished = false;
            StartCoroutine(TaskListCoroutine());
        }

        /// <summary>
        /// serialization interface implementation
        /// </summary>
        public void OnBeforeSerialize()
        {
            serializedTasks.Clear();
            foreach (var task in Tasks)
                serializedTasks.Add(new SerializableTask(task));
        }

        /// <summary>
        /// create a task from a serializable task
        /// </summary>
        public Task ReadTaskFromSerializedTask(SerializableTask st)
        {
            switch (st.TypeName)
            {
                case "MovingTask":
                    {
                        return new MovingTask(st);
                    }break;
                case "ClickingTask":
                    {
                        return new ClickingTask(st);
                    }break;
                default:
                    {
                        return new Task(st);
                    }
            }
        }

        /// <summary>
        /// serialization interface implementation
        /// </summary>
        public void OnAfterDeserialize()
        {
            Tasks.Clear();
            foreach (var serializedTask in serializedTasks)
                Tasks.Add(ReadTaskFromSerializedTask(serializedTask));
        }

        /// <summary>
        /// at start, after the multiparts object awake method, we setup the necessary things
        /// </summary>
        public void Start()
        {
            // make sure we have a multiparts object
            MultiPartsObject mpo = GetComponent<MultiPartsObject>();
            if (mpo == null)
                Debug.LogError("No multiparts object attached");

            mpo.OnReleaseEvent.AddListener(CheckTaskOnRelease);
            mpo.OnSelectEvent.AddListener(CheckTaskOnSelect);

            StoreInitialPosAndRot();
            // register each task "CheckTask" function to onrelease of the appropriate node
            StartCoroutine(TaskListCoroutine());
        }

        public IEnumerator TaskListCoroutine()
        {
            // set current task id to -1 as we're going to increment by 1 on NextTask() on start
            CurrentTaskId = -1;

            // lock all nodes
            ChangeLockOfAllTaskNodes(true);

            foreach (var task in Tasks)
            {
                // destroy previous hint if any
                if (Hint != null)
                    Destroy(Hint);

                // increment task id
                CurrentTaskId++;

                // unlock node
                ChangeLockOfTaskNode(task, false);

                // fire task start event
                if (TaskStartListeners != null)
                    TaskStartListeners.Invoke(Tasks[CurrentTaskId]);

                // draw current task hint
                Tasks[CurrentTaskId].DrawTaskHint(this);
                while (!Tasks[CurrentTaskId].Finished)
                {
                    Tasks[CurrentTaskId].UpdateTaskHint(this);
                    yield return null;
                }

                MultiPartsObject mpo = GetComponent<MultiPartsObject>();
                mpo.Release();
                mpo.Deselect(Tasks[CurrentTaskId].GameObject);

                // lock node again
                ChangeLockOfTaskNode(task, true);

                // destroy hint if any
                if (Hint != null)
                    Destroy(Hint);

                // run task event coroutine if exists
                if (Tasks[CurrentTaskId].TaskEvent != null)
                    yield return StartCoroutine(Tasks[CurrentTaskId].TaskEvent.TaskEventCoroutine(mpo));
            }
            yield return null;
        }

        void ChangeLockOfTaskNode(Task task, bool value)
        {
            MultiPartsObject mpo = GetComponent<MultiPartsObject>();
            if (mpo == null)
                Debug.LogError("No multiparts object attached");
            else
            {
                Node node = null;
                if(mpo.Dict.TryGetValue(task.GameObject,out node))
                    node.Locked = value;
            }
        }

        void ChangeLockOfAllTaskNodes(bool value)
        {
            foreach (var task in Tasks)
                ChangeLockOfTaskNode(task, value);
        }

        void OnDrawGizmosSelected()
        {
            if (Tasks.Count > CurrentTaskId && CurrentTaskId >= 0)
            {
                Tasks[CurrentTaskId].DrawEditorTaskHint();
            }
        }

        /// <summary>
        /// fires check task for moving task
        /// </summary>
        public void CheckTaskOnRelease(Node node)
        {
            if(CurrentTaskId < Tasks.Count && CurrentTaskId != -1)
            {
                Task task = Tasks[CurrentTaskId];
                if (task.GetType().Name != "MovingTask") return;
                if (task.GameObject == node.GameObject)
                {
                    // make sure we have a multiparts object
                    MultiPartsObject mpo = GetComponent<MultiPartsObject>();
                    if (mpo == null)
                        Debug.LogError("No multiparts object attached");
                    task.CheckTask(mpo);
                }
            }
        }

        /// <summary>
        /// fires check task for clicking task
        /// </summary>
        public void CheckTaskOnSelect(Node node)
        {
            if (CurrentTaskId < Tasks.Count && CurrentTaskId != -1)
            {
                Task task = Tasks[CurrentTaskId];
                if (task.GetType().Name != "ClickingTask") return;
                if (task.GameObject == node.GameObject)
                {
                    // make sure we have a multiparts object
                    MultiPartsObject mpo = GetComponent<MultiPartsObject>();
                    if (mpo == null)
                        Debug.LogError("No multiparts object attached");
                    task.CheckTask(mpo);
                }
            }
        }
    }
}
