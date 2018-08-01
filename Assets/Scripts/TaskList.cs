/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    public class TaskList : MonoBehaviour, ISerializationCallbackReceiver
    {
        private int _currentTaskId = 0;
        public int CurrentTaskId
        {
            get { return _currentTaskId; }
            set { _currentTaskId = value; }
        }

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
        /// the serialized node structure
        /// </summary>
        public List<SerializableTask> serializedTasks;

        public void OnBeforeSerialize()
        {
            serializedTasks.Clear();
            foreach (var task in Tasks)
                serializedTasks.Add(new SerializableTask(task));
        }

        public Task ReadTaskFromSerializedTask(SerializableTask st)
        {
            switch (st.TypeName)
            {
                case "MovingTask":
                    {
                        return new MovingTask(st);
                    }break;
            }
            return new Task(st);
        }

        public void OnAfterDeserialize()
        {
            Tasks.Clear();
            foreach (var serializedTask in serializedTasks)
                Tasks.Add(ReadTaskFromSerializedTask(serializedTask));
        }

        public void Awake()
        {
            MultiPartsObject mpo = GetComponent<MultiPartsObject>();
            if (mpo == null)
                Debug.LogError("No multiparts object attached");

            CurrentTaskId = 0;

            foreach(var task in Tasks)
            {
                task.TaskList = this;
                Node node = null;
                if(mpo.Dict.TryGetValue(task.GameObject,out node))
                {
                    node.OnReleaseEvent.AddListener(task.CheckTask);
                }
            }
        }
    }
}
