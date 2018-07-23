using HoloToolkit.Unity.InputModule;
using ModelViewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HololensInterfacePickable : MonoBehaviour, IInputClickHandler,IInputHandler
{
    public HololensInterface hi;
    public MultiPartsObject mpo;
    public bool grabbing = false;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (hi.mode == 1)
        {
            mpo.ToggleSelect(eventData.selectedObject);
        }/*else if(hi.mode == 2)
        {
            Debug.Log("mode is 2");
            if (!grabbing)
            {
                Debug.Log("mode is 2 grabbing false");
                grabbing = true;
                mpo.Grab();
            }
            else
            {
                Debug.Log("mode is 2 grabbing true");
                grabbing = false;
                mpo.Release();
            }
                
        }*/
    }

    public void OnInputDown(InputEventData eventData)
    {
        if (hi.mode == 2)
        {
                grabbing = true;
                mpo.Grab();

        }
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (hi.mode == 2)
        {
                grabbing = false;
                mpo.Release();
        }
    }
}
