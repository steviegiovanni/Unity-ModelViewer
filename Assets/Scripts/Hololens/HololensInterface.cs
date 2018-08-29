using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using ModelViewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HololensInterface : MonoBehaviour, IInputClickHandler {
    public TaskList TaskList;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log(eventData.selectedObject.name);
        if(eventData.selectedObject.name == "Disassembly")
        {
            var go = FindObjectOfType<AnimatedCursor>();
            if (go != null)
                Destroy(go.gameObject);
            var go1 = FindObjectOfType<MotionControllerVisualizer>();
            if (go1 != null)
                Destroy(go1.gameObject);
            var go2 = FindObjectOfType<MixedRealityCameraManager>();
            if (go2 != null)
                Destroy(go2.gameObject);
            var go3 = FindObjectOfType<FocusManager>();
            if (go3 != null)
                Destroy(go3.gameObject);
            var go4 = FindObjectOfType<StabilizationPlaneModifier>();
            if (go4 != null)
                Destroy(go4.gameObject);
            var go5 = FindObjectOfType<InputManager>();
            if (go5 != null)
                Destroy(go5.gameObject);
            var go6 = FindObjectOfType<GazeManager>();
            if (go6 != null)
                Destroy(go6.gameObject);
            SceneManager.LoadScene("DynamicContent-hololens-disassembly");
        }
        else if (eventData.selectedObject.name == "ToggleGrab")
        {
        }
        else if(eventData.selectedObject.name == "Reset")
        {
            TaskList.Reset();
        }
    }

    private void Update()
    {
        
    }
}
