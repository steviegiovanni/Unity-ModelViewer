/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// simple UI, listens to on task start event and shows the task name in front of the user for each task 
    /// </summary>
    public class TaskListUI : MonoBehaviour
    {
        /// <summary>
        /// the task list to listen to
        /// </summary>
        [SerializeField]
        private TaskList _taskList;
        public TaskList TaskList
        {
            get { return _taskList; }
            set { _taskList = value; }
        }

        /// <summary>
        /// the task name will only be shown for this short duration of time
        /// </summary>
        [SerializeField]
        private float _hintDuration = 3.0f;
        public float HintDuration
        {
            get { return _hintDuration; }
            set { _hintDuration = value; }
        }

        /// <summary>
        /// the text mesh prefab to be used to show the task name
        /// </summary>
        [SerializeField]
        private GameObject _hintPrefab;

        /// <summary>
        /// the currently instantiated hint
        /// </summary>
        private GameObject hintObject;

        [SerializeField]
        private GameObject hintText;

        private void Awake()
        {
            // listen to task list start task event
            TaskList.TaskStartListeners.AddListener(ShowNextTask);
        }

        private void Update()
        {
            // update elapsed time, if it's more than duration, destroys the hint
            /*elapsedTime += Time.deltaTime;
            if (HintDuration <= elapsedTime && hintObject != null)
                Destroy(hintObject);*/
        }

        /// <summary>
        /// the function to be called when this object receives the start task event
        /// </summary>
        public void ShowNextTask(Task task)
        {
            /*elapsedTime = 0.0f;
            //hintObject = Instantiate(_hintPrefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);
            if (hintObject != null) Destroy(hintObject);
            hintObject = Instantiate(_hintPrefab, Camera.main.transform.position + (this.transform.position - Camera.main.transform.position).normalized, Quaternion.identity);
            hintObject.transform.LookAt(2 * hintObject.transform.position - Camera.main.transform.position);
            hintObject.GetComponent<TextMesh>().text = task.TaskName;*/
            if(task == null)
                hintText.GetComponent<TextMesh>().text = "Well Done!";
            else
                hintText.GetComponent<TextMesh>().text = task.TaskName;
        }
    }
}
