/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    public class TaskListUI : MonoBehaviour
    {
        [SerializeField]
        private TaskList _taskList;
        public TaskList TaskList
        {
            get { return _taskList; }
            set { _taskList = value; }
        }

        [SerializeField]
        private float _hintDuration = 3.0f;
        public float HintDuration
        {
            get { return _hintDuration; }
            set { _hintDuration = value; }
        }

        [SerializeField]
        private GameObject _hintPrefab;

        private float elapsedTime = 0.0f;
        private GameObject hintObject;

        private void Awake()
        {
            TaskList.TaskStartListeners.AddListener(ShowNextTask);
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (HintDuration <= elapsedTime && hintObject != null)
                Destroy(hintObject);
        }

        public void ShowNextTask(Task task)
        {
            elapsedTime = 0.0f;
            hintObject = Instantiate(_hintPrefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);
            hintObject.transform.LookAt(2 * hintObject.transform.position - Camera.main.transform.position);
            hintObject.GetComponent<TextMesh>().text = task.TaskName;
        }
    }
}
