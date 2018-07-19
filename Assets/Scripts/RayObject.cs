using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayObject : MonoBehaviour, IHasRay
{
    public Ray GetRay()
    {
        return new Ray(Camera.main.transform.position, transform.forward);
    }
}
