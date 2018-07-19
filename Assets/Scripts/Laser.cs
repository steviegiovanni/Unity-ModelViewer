using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out hitInfo,
            20.0f,
            Physics.DefaultRaycastLayers))
        {
            //Debug.Log(hitInfo.collider.gameObject.name);
        }
    }
}
