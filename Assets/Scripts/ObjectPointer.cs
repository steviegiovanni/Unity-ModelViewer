/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

/// <summary>
/// interface for everything that uses a ray
/// implements this interface for different devices (e.g. hololens, vive)
/// </summary>
public interface IHasRay
{
    Ray GetRay();
}

/// <summary>
/// Object pointer returns the hitinfo of a specified ray in the simulation
/// implemented as singleton to handle situation where users forgot to put one in the scene
/// as this will be used by the multipartsobject component
/// </summary>
public class ObjectPointer : Singleton<ObjectPointer> {
    /// <summary>
    /// the ray used to point to object
	/// need to be assigned accordingly whether u're using the ray from hololens' headset, vive's controller etc...
    /// </summary>
    [SerializeField]
    private GameObject _ray;
    private IHasRay _iray;
    public IHasRay Ray
    {
        get { return _iray; }
        set { _iray = value; }
    }

	/// <summary>
	/// line renderer to show ray
	/// </summary>
	private LineRenderer _lr;
	public LineRenderer LR{
		get{
			if (_lr == null)
				_lr = GetComponent<LineRenderer> ();
			return _lr;
		}
	}

	/// <summary>
	/// show the ray or not
	/// </summary>
	[SerializeField]
	private bool _rayVisible;
	public bool RayVisible{
		get{ return _rayVisible; }set{ _rayVisible = value;}
	}

    void OnValidate()
    {
        if (_ray != null)
        {
            _iray = _ray.GetComponent<IHasRay>();
            if (_iray == null) _ray = null;
        }
    }

    /// <summary>
    /// the hit info
    /// </summary>
    private RaycastHit _hitInfo;
    public RaycastHit HitInfo
    {
        get { return _hitInfo; }
    }

    // Update is called once per frame
    void Update () {
        // use default ray with no dir if there's no HasRay object
        Ray ray;
        if (Ray == null)
            ray = new Ray(Vector3.zero, Vector3.zero);
        else
            ray = Ray.GetRay();

		// draw ray if there's a line renderer and ray is visible
		if (LR != null) {
			if (RayVisible) {
				LR.SetPosition (0, ray.origin);
				LR.SetPosition (1, ray.origin + ray.direction * 10);
			} else {
				LR.enabled = false;
			}
		}

        // raycast to get hitInfo
        if (Physics.Raycast(ray, out _hitInfo, 20.0f, Physics.DefaultRaycastLayers))
            Debug.Log(_hitInfo.collider.gameObject.name);
	}
}
