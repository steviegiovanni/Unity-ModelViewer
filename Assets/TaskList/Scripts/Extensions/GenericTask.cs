using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// represents a generic task. can be used when user selects a component and stuff need to happen
    /// </summary>
    public class GenericTask : Task
    {
        private TaskEvent _taskEvent;
        public TaskEvent TaskEvent
        {
            get { return _taskEvent; }
            set { _taskEvent = value; }
        }

        /// <summary>
        /// constructor taking a go
        /// </summary>
        public GenericTask(GameObject go) : base(go){
            _taskEvent = null;
        }

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public GenericTask (SerializableTask task):base(task)
        {
            SerializableTaskEvent ste = task.TaskEvent;
            if (ste != null)
            {
                switch (ste.TypeName)
                {
                    case "TransformTaskEvent":
                        {
                            TaskEvent = new TransformTaskEvent(ste);
                        }break;
                }
            }
        }
    }
}
