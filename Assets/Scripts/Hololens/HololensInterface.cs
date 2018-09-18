using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using ModelViewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HololensInterface : MonoBehaviour, IInputClickHandler {
    public TaskList TaskList;
    public MultiPartsObject MPO;
    public HoloInterface HoloInterface;
    public GameObject PartButton;
    public GameObject WholeButton;
    public GameObject UIInFrontButton;
    public GameObject ObjectInFrontButton;
    public MultiPartsObjectCageUI mpoCageUI;

    public void CleanUpHololensPrefabs()
    {
        var go = GameObject.Find("MixedRealityCameraParent");
        if (go != null)
            Destroy(go);
        go = GameObject.Find("DefaultCursor");
        if (go != null)
            Destroy(go);
        go = GameObject.Find("InputManager");
        if (go != null)
            Destroy(go);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (eventData == null) return;
        //Debug.Log(eventData.selectedObject.name);

        if(eventData.selectedObject.name == "TimingBeltTask")
        {
            CleanUpHololensPrefabs();
            SceneManager.LoadScene("DynamicContent-hololens-timingbelt");
        }
        else if (eventData.selectedObject.name == "HeadGasketTask")
        {
            CleanUpHololensPrefabs();
            SceneManager.LoadScene("DynamicContent-hololens-headgasket");
        }
        else if (eventData.selectedObject.name == "Inspect")
        {
            CleanUpHololensPrefabs();
            SceneManager.LoadScene("DynamicContent-hololens-inspect");
        }
        else if(eventData.selectedObject.name == "Reset")
        {
            TaskList.Reset();
        }
        else if (eventData.selectedObject.name == "ResetMPO")
        {
            MPO.ResetAll(MPO.Root);
        }
        else if (eventData.selectedObject.name == "Part")
        {
            HoloInterface.partOrWhole = false;
            WholeButton.SetActive(true);
            PartButton.SetActive(false);
        }
        else if (eventData.selectedObject.name == "Whole")
        {
            HoloInterface.partOrWhole = true;
            WholeButton.SetActive(false);
            PartButton.SetActive(true);
        }
        else if(eventData.selectedObject.name == "UIInFront")
        {
            UIInFrontButton.SetActive(false);
            ObjectInFrontButton.SetActive(true);
            mpoCageUI.BringUICloser();
        }
        else if (eventData.selectedObject.name == "ObjectInFront")
        {
            UIInFrontButton.SetActive(true);
            ObjectInFrontButton.SetActive(false);
            mpoCageUI.BringUIFurther();
        }
    }

    void Start()
    {
        WholeButton.SetActive(true);
        PartButton.SetActive(false);
        UIInFrontButton.SetActive(true);
        ObjectInFrontButton.SetActive(false);
    }

    private void Update()
    {
        
    }
}
