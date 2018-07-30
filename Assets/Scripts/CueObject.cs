/// author: Stevie Giovanni

using UnityEngine;

/// <summary>
/// just a simple class to destroy the GO this object is attached to after a short time
/// scales the object as well as time goes by
/// </summary>
public class CueObject : MonoBehaviour {
    /// <summary>
    /// lifetime of the object
    /// </summary>
    [SerializeField]
    private float _lifetime = 3.0f;
    public float Lifetime
    {
        get { return _lifetime; }
        set { _lifetime = value; }
    }

    /// <summary>
    /// current time
    /// </summary>
    private float elapsedTime;
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= Lifetime)
            Destroy(this.gameObject);
        else
        {
            // scale the object bigger as time goes by
            this.transform.localScale += Vector3.one * Time.deltaTime;
        }
	}
}
