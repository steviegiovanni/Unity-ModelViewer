using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a node represents an element in a multipartsobject
/// </summary>
public class Node
{
    private GameObject _gameObject;
    public GameObject GameObject
    {
        get { return _gameObject; }
        set { _gameObject = value; }
    }

    private bool _hasMesh = false;
    public bool HasMesh
    {
        get { return _hasMesh; }
        set { _hasMesh = value; }
    }

    private List<Node> _childs;
    public List<Node> Childs
    {
        get
        {
            if (_childs == null)
                _childs = new List<Node>();
            return _childs;
        }
        set { _childs = value; }
    }

    public Node()
    {
        GameObject = null;
        HasMesh = false;
        Childs = new List<Node>();
    }

    public Node(GameObject go)
    {
        // assign game object
        GameObject = go;

        // check whether game object has a mesh
        HasMesh = go.GetComponent<MeshFilter>() != null;

        // check childs
        foreach(Transform child in go.transform){
            Childs.Add(new Node(child.gameObject));
        }
    }
}

public class MultiPartsObject : MonoBehaviour {
    private Node _root = null;
    public Node Root
    {
        get { return _root; }
        set { _root = value; }
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnDrawGizmos()
    {
        // construct hierarchy
        if(Root == null)
            Root = new Node(this.gameObject);

        // draw gizmos for each node in the hierarchy
        DrawNode(Root);
    }

    public void DrawNode(Node node)
    {
        if(node != null)
        {
            // draw origin
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.GameObject.transform.position,0.1f);

            // draw bounding box if any
            if (node.HasMesh) {
                Gizmos.color = Color.green;
                Renderer rend = node.GameObject.GetComponent<Renderer>();
                Mesh mesh = node.GameObject.GetComponent<MeshFilter>().sharedMesh;
                Bounds bounds = rend.bounds;
                //Bounds bounds = mesh.bounds;
                Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
                //Gizmos.DrawWireSphere(rend.bounds.center,rend.bounds.extents.magnitude);
            }

            foreach(var child in node.Childs)
            {
                DrawNode(child);
            }
        }
    }
}
