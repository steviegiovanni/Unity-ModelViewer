/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModelViewer;

/// <summary>
/// Helper class, get a reference to a multipartsobject and instantiate 
/// cues to indicate currently scattered part one bye one
/// </summary>
public class Sonar : MonoBehaviour
{
    /// <summary>
    /// the multiparts object
    /// </summary>
    [SerializeField]
    private MultiPartsObject _mpo;
    public MultiPartsObject MPO
    {
        get { return _mpo; }
        set { _mpo = value; }
    }

    /// <summary>
    /// cooldown before instantiating another signal
    /// </summary>
    [SerializeField]
    private float _cooldown = 5.0f;
    public float Cooldown {
        get { return _cooldown; }
        set { _cooldown = value; }
    }

    /// <summary>
    /// current tick
    /// </summary>
    private float elapsed = 0.0f;

    /// <summary>
    /// the prefab that will be instantiated
    /// </summary>
    [SerializeField]
    private GameObject _signal;
    public GameObject Signal
    {
        get { return _signal; }
        set { _signal = value; }
    }

    public void Update()
    {
        // if the MPO root is null, no need to go through this
        if (MPO.Root == null) return;

        elapsed += Time.deltaTime;
        if (elapsed >= Cooldown)
        {
            elapsed = 0.0f;

            Node foundNode = FindComponent();
            if((foundNode != null) && (Signal != null))
                Instantiate(Signal, foundNode.GameObject.transform.position, foundNode.GameObject.transform.rotation);

        }
    }

    /// <summary>
    /// find the first scattered component in the tree of the MPO
    /// </summary>
    public Node FindComponent()
    {
        Node retval = null;
        List<Node> toCheck = new List<Node>();
        toCheck.Add(MPO.Root);
        bool found = false;
        while (toCheck.Count > 0 && !found)
        {
            Node curNode = toCheck[0];
            Vector3 oriPosAfterSetup = MPO.transform.TransformPoint(curNode.P0);
            if (curNode.HasMesh && Vector3.Distance(curNode.GameObject.transform.position, oriPosAfterSetup) >= 0.1f)
            {
                found = true;
                break;
            }
            else
            {
                toCheck.Remove(curNode);
                foreach (var child in curNode.Childs)
                    toCheck.Add(child);
            }
        }
        if (found)
            retval = toCheck[0];
        return retval;
    }
}
