/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interface for everything that uses a ray to interact with objects
/// </summary>
public interface IHasRay
{
    Ray GetRay();
}

public class ObjectPointer : Singleton<ObjectPointer> {
    /// <summary>
    /// the ray used to point to object
    /// </summary>
    [SerializeField]
    private GameObject _ray;
    private IHasRay _iray;
    public IHasRay Ray
    {
        get { return _iray; }
        set { _iray = value; }
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

        // raycast!
        if (Physics.Raycast(ray, out _hitInfo, 20.0f, Physics.DefaultRaycastLayers))
        {
            Debug.Log(_hitInfo.collider.gameObject.name);
        }
        else
        {
            //Debug.Log("doesn't hit anything");
        }
	}
}
