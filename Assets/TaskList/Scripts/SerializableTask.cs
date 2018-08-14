﻿/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    [System.Serializable]
    public class SerializableTask
    {
        public string TypeName;
        public GameObject GameObject;
        public Vector3 Position;
        public Quaternion Rotation;
        public float SnapThreshold;
        public string TaskName;
        public string Description;
        public MovingTaskType MoveType;
        
        public SerializableTask(Task t)
        {
            TypeName = t.GetType().Name;
            GameObject = t.GameObject;
            TaskName = t.TaskName;
            Description = t.Description;
            switch (TypeName)
            {
                case "MovingTask":
                    {
                        MovingTask castedTask = (MovingTask)t;
                        Position = castedTask.Position;
                        Rotation = castedTask.Rotation;
                        SnapThreshold = castedTask.SnapThreshold;
                        MoveType = castedTask.MoveType;
                    }break;
            }
        }
    }
}
