using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    public class TransformTaskEvent : TaskEvent
    {
        private Vector3 _startPos;
        public Vector3 StartPos
        {
            get { return _startPos; }
            set { _startPos = value; }
        }

        private Vector3 _endPos;
        public Vector3 EndPos
        {
            get { return _endPos; }
            set { _endPos = value; }
        }

        private Quaternion _startRotation;
        public Quaternion StartRotation
        {
            get { return _startRotation; }
            set { _startRotation = value; }
        }

        private Quaternion _endRotation;
        public Quaternion EndRotation
        {
            get { return _endRotation; }
            set { _endRotation = value; }
        }

        public TransformTaskEvent(SerializableTaskEvent ste) : base(ste) {
            StartPos = ste.StartPos;
            EndPos = ste.EndPos;
            StartRotation = ste.StartRotation;
            EndRotation = ste.EndRotation;
        }

        public TransformTaskEvent() : base() { }
    }
}
