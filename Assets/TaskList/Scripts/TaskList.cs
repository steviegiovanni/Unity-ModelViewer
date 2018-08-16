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
                case "GenericTask":
                    {
                        return new GenericTask(st);
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

            // set current task id to -1 as we're going to increment by 1 on NextTask() on start
            CurrentTaskId = -1;

            // register each task "CheckTask" function to onrelease of the appropriate node
            foreach(var task in Tasks)
            {
                // set task's owner to this tasklist
                task.TaskList = this;
                Node node = null;
                if(mpo.Dict.TryGetValue(task.GameObject,out node))
                {
                    node.OnReleaseEvent.AddListener(task.CheckTask);
                }
            }

            // start next task at current id 0
            NextTask();
        }

        // next task is called by each task when it is finished
        public void NextTask()
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
                Tasks[CurrentTaskId].DrawTaskHint();
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Tasks.Count > CurrentTaskId && CurrentTaskId >= 0)
            {
                Tasks[CurrentTaskId].DrawEditorTaskHint();
            }
        }
    }
}
