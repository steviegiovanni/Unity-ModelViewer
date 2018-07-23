using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModelViewer;

/// <summary>
/// interfaces Steam input with MultiPartsObject's interaction
/// </summary>
public class SteamInterface : MonoBehaviour {
	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;

	/// <summary>
	/// the multiparts object component
	/// </summary>
	[SerializeField]
	private MultiPartsObject mpo;

	// Use this for initialization
	void Start () {
		trackedObject = GetComponent<SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		device = SteamVR_Controller.Input ((int)trackedObject.index);
		/*if (device.GetHairTriggerDown()) {
			mpo.ToggleSelect ();
		}*/

		if (device.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad)) {
			mpo.ToggleSelect ();
		} else if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
			mpo.GrabIfPointingAt ();
		} else if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
			mpo.Release ();
		} else if (device.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
			//mpo.Grab ();
		} else if (device.GetPressUp (SteamVR_Controller.ButtonMask.Grip)) {
			//mpo.Release();
			mpo.ResetTransform (mpo.Root);
			mpo.FitToScale (mpo.Root, mpo.VirtualScale);
		}
	}
}
