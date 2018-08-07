using HoloToolkit.Unity.InputModule;
using ModelViewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HololensInterface : MonoBehaviour, IInputClickHandler {
    public int mode = 0;

    public GameObject selectButton;
    public GameObject grabButton;
    public GameObject resetButton;

    public MultiPartsObject mpo;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log(eventData.selectedObject.name);
        if(eventData.selectedObject.name == "ToggleSelect")
        {
            if (mode == 1)
                mode = 0;
            else
                mode = 1;
        }else if (eventData.selectedObject.name == "ToggleGrab")
        {
            if (mode == 2)
                mode = 0;
            else
                mode = 2;
        }
        else if(eventData.selectedObject.name == "Reset")
        {
            mpo.ResetAll(mpo.Root);
            mpo.FitToScale(mpo.Root, mpo.VirtualScale);
        }
    }

    private void Update()
    {
        switch (mode)
        {
            case 0: {
                    selectButton.GetComponent<Renderer>().material.color = Color.white;
                    grabButton.GetComponent<Renderer>().material.color = Color.white;
                } break;
            case 1: {
                    selectButton.GetComponent<Renderer>().material.color = Color.red;
                    grabButton.GetComponent<Renderer>().material.color = Color.white;
                } break;
            case 2: {
                    selectButton.GetComponent<Renderer>().material.color = Color.white;
                    grabButton.GetComponent<Renderer>().material.color = Color.red;
                } break;
        }
    }
}
