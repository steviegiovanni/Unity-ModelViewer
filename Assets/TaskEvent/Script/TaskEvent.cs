using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    public class TaskEvent 
    {
        private Task _task;
        public Task Task
        {
            get { return _task; }
            set { _task = value; }
        }

        public TaskEvent() { }
        public TaskEvent(SerializableTaskEvent serializableTaskEvent) { }

        public virtual IEnumerator TaskEventCoroutine()
        {
            yield return null;
        }
    }
}
