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

        private float _duration = 3.0f;
        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public TransformTaskEvent(SerializableTaskEvent ste) : base(ste) {
            StartPos = ste.StartPos;
            EndPos = ste.EndPos;
            StartRotation = ste.StartRotation;
            EndRotation = ste.EndRotation;
        }

        public TransformTaskEvent() : base() { }

        public override IEnumerator TaskEventCoroutine()
        {
            Debug.Log("Transform Event Coroutine reached");

            List<Vector3> childStartPositions = new List<Vector3>();
            List<Quaternion> childStartRotations = new List<Quaternion>();

            GameObject taskObj = Task.GameObject;
            foreach(Transform child in taskObj.transform)
            {
                childStartPositions.Add(child.position);
                childStartRotations.Add(child.rotation);
            }

            float startTime = Time.time;
            float curTime = startTime;
            while(curTime - startTime < Duration)
            {
                curTime += Time.deltaTime;

                taskObj.transform.SetPositionAndRotation(taskObj.transform.position + (EndPos - StartPos) * Time.deltaTime / Duration, Quaternion.Lerp(StartRotation, EndRotation, (curTime - startTime) / Duration));

                for (int i = 0; i < taskObj.transform.childCount; i++)
                    taskObj.transform.GetChild(i).SetPositionAndRotation(childStartPositions[i],childStartRotations[i]);

                yield return null;
            }

            yield return null;
        }
    }
}
