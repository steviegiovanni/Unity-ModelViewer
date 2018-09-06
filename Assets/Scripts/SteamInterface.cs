using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModelViewer;
using UnityEngine.SceneManagement;

/// <summary>
/// interfaces Steam input with MultiPartsObject's interaction
/// </summary>
public class SteamInterface : MonoBehaviour {
    [SerializeField]
	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;

    public GameObject WholeButton;
    public GameObject PartButton;

    private bool PartOrWhole = false;

	/// <summary>
	/// the multiparts object component
	/// </summary>
	[SerializeField]
	private MultiPartsObject mpo;

    [SerializeField]
    private TaskList taskList;

	// Use this for initialization
	void Start () {
		//trackedObject = GetComponent<SteamVR_TrackedObject> ();
        WholeButton.SetActive(true);
        PartButton.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (trackedObject.index < 0) return;
		device = SteamVR_Controller.Input ((int)trackedObject.index);

        GameObject hitObject = ObjectPointer.Instance.HitInfo.collider!=null? ObjectPointer.Instance.HitInfo.collider.gameObject:null;
        if (hitObject != null) {
            switch (hitObject.name)
            {
                case "Reset":
                    {
                        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                            taskList.Reset();
                    }break;
                case "ResetMPO":
                    {
                        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                            mpo.ResetAll(mpo.Root);
                    }break;
                case "Inspect":
                    {
                        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                            SceneManager.LoadScene("DynamicContent-vive-inspect");
                    }
                    break;
                case "TimingBeltTask":
                    {
                        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                            SceneManager.LoadScene("DynamicContent-vive-timingbelt");
                    }break;
                case "HeadGasketTask":
                    {
                        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                            SceneManager.LoadScene("DynamicContent-vive-headgasket");
                    }
                    break;
                case "Part":
                    {
                        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                        {
                            PartOrWhole = false;
                            WholeButton.SetActive(true);
                            PartButton.SetActive(false);
                        }
                    }
                    break;
                case "Whole":
                    {
                        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) {
                            PartOrWhole = true;
                            WholeButton.SetActive(false);
                            PartButton.SetActive(true);
                        }
                    }
                    break;
            }
        }

        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            mpo.ToggleSelect();
        }
        else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (PartOrWhole)
                mpo.GrabCage();
            else
                mpo.GrabIfPointingAt();
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (PartOrWhole)
                mpo.ReleaseCage();
            else
                mpo.Release();
        }
        else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            //mpo.Grab ();
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            //mpo.Release();
            mpo.ResetAll(mpo.Root);
            //mpo.FitToScale (mpo.Root, mpo.VirtualScale);
        }
    }
}
