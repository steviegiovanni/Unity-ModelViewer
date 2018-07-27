using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour {
    public float lifetime = 0.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        lifetime += Time.deltaTime;
        if (lifetime >= 3.0f)
            Destroy(this.gameObject);
        else
        {
            this.transform.localScale += Vector3.one * Time.deltaTime;
        }
	}
}
