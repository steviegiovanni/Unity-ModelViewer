using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    [System.Serializable]
    public class SerializableTaskEvent
    {
        public string TypeName;
        public Vector3 StartPos;
        public Quaternion StartRotation;
        public Vector3 EndPos;
        public Quaternion EndRotation;

        public SerializableTaskEvent(TaskEvent taskEvent) {
            if (taskEvent != null) {
                TypeName = taskEvent.GetType().Name;
                switch (TypeName)
                {
                    case "TransformTaskEvent":
                        {
                            TransformTaskEvent castedEvent = taskEvent as TransformTaskEvent;
                            StartPos = castedEvent.StartPos;
                            EndPos = castedEvent.EndPos;
                            StartRotation = castedEvent.StartRotation;
                            EndRotation = castedEvent.EndRotation;
                        }break;
                }
            }
        }
    }
}
