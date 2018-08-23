/// author: Stevie Giovanni

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
            // register each task "CheckTask" function to onrelease of the appropriate node
            StartCoroutine(TaskListCoroutine());
        }

        public IEnumerator TaskListCoroutine()
        {
            // set current task id to -1 as we're going to increment by 1 on NextTask() on start
            CurrentTaskId = -1;

            foreach (var task in Tasks)
            {
                // destroy previous hint if any
                if (Hint != null)
                    Destroy(Hint);

                // increment task id
                CurrentTaskId++;

                // fire task start event
                if (TaskStartListeners != null)
                    TaskStartListeners.Invoke(Tasks[CurrentTaskId]);

                // draw current task hint
                Tasks[CurrentTaskId].DrawTaskHint(this);
                while (!Tasks[CurrentTaskId].Finished)
                    yield return null;
                MultiPartsObject mpo = GetComponent<MultiPartsObject>();
                mpo.Release();
                mpo.Deselect(Tasks[CurrentTaskId].GameObject);

                // destroy hint if any
                if (Hint != null)
                    Destroy(Hint);

                // run task event coroutine if exists
                if (Tasks[CurrentTaskId].TaskEvent != null)
                    yield return StartCoroutine(Tasks[CurrentTaskId].TaskEvent.TaskEventCoroutine());
            }
            yield return null;
        }

        // next task is called by each task when it is finished
        /*public void NextTask()
        {
            // if not task 0, there's a previous task, do some cleanup, locked the node etc.
            if (Tasks.Count <= 0) return;
            MultiPartsObject mpo = GetComponent<MultiPartsObject>();
            if (CurrentTaskId != -1)
            {
                Node node = null;
                if (mpo.Dict.TryGetValue(Tasks[CurrentTaskId].GameObject, out node))
                    node.Locked = true;
            }

            // increment current task id
            CurrentTaskId++;
            // destroy previous hint if any
            if (Hint != null)
                Destroy(Hint);

            // if not exceeding tasks count, there's a new task, set it up. unlock the node, setup hint etc.
            if (CurrentTaskId < Tasks.Count)
            {
                Node node = null;
                if (mpo.Dict.TryGetValue(Tasks[CurrentTaskId].GameObject, out node)) {
                    node.Locked = false;
                }

                // fire task start event
                if (TaskStartListeners != null)
                    TaskStartListeners.Invoke(Tasks[CurrentTaskId]);

                // leave the task to draw the next hint to each task
                Tasks[CurrentTaskId].DrawTaskHint(this);
            }
        }*/

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
                    task.CheckTask();
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
                    task.CheckTask();
            }
        }
    }
}
