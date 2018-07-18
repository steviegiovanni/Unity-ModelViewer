/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a node represents an element in a multipartsobject
/// </summary>
public class Node
{
    /// <summary>
    /// the game object associated with this node
    /// </summary>
    private GameObject _gameObject;
    public GameObject GameObject
    {
        get { return _gameObject; }
        set { _gameObject = value; }
    }

    /// <summary>
    /// whether this node has a mesh or only a placeholder transform
    /// </summary>
    private bool _hasMesh = false;
    public bool HasMesh
    {
        get { return _hasMesh; }
        set { _hasMesh = value; }
    }

    /// <summary>
    /// the parent node of the node
    /// </summary>
    private Node _parent;
    public Node Parent
    {
        get { return _parent; }
        set { _parent = value; }
    }

    /// <summary>
    /// childs of the node
    /// </summary>
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

    /// <summary>
    /// the original position of the node
    /// </summary>
    private Vector3 _p0;
    public Vector3 P0
    {
        get { return _p0; }
        set { _p0 = value; }
    }

    /// <summary>
    /// the original rotation of the node
    /// </summary>
    private Quaternion _r0;
    public Quaternion R0
    {
        get { return _r0; }
        set { _r0 = value; }
    }

    /// <summary>
    /// the original scale of the node
    /// </summary>
    private Vector3 _s0;
    public Vector3 S0
    {
        get { return _s0; }
        set { _s0 = value; }
    }

    /// <summary>
    /// default constructor
    /// </summary>
    public Node()
    {
        GameObject = null;
        HasMesh = false;
        P0 = Vector3.zero;
        R0 = Quaternion.identity;
        S0 = Vector3.one;

        Childs = new List<Node>();
    }

    /// <summary>
    /// constructor that takes a go and a parent node
    /// </summary>
    public Node(GameObject go, Node parent)
    {
        // assign parent
        Parent = parent;

        // assign game object
        GameObject = go;

        // get the original transforms
        P0 = go.transform.position;
        R0 = go.transform.rotation;
        S0 = go.transform.localScale;

        // check whether game object has a mesh
        HasMesh = go.GetComponent<MeshFilter>() != null;

        // check childs
        foreach (Transform child in go.transform)
        {
            Childs.Add(new Node(child.gameObject,this));
        }
    }
}

/// <summary>
/// a multi parts object is an object that consists of several different parts
/// that can be detached and assembled back again
/// </summary>
public class MultiPartsObject : MonoBehaviour {
    /// <summary>
    /// the root node
    /// </summary>
    private Node _root = null;
    public Node Root
    {
        get { return _root; }
        set { _root = value; }
    }

    /// <summary>
    /// mapping between a game object and a node
    /// </summary>
    private Dictionary<GameObject, Node> _dict;
    public Dictionary<GameObject, Node> Dict
    {
        get {
            if (_dict == null)
                _dict = new Dictionary<GameObject, Node>();
            return _dict;
        }
    }

    /// <summary>
    /// the virtual size of the object
    /// </summary>
    [Range(0.0f,10.0f)]
    [SerializeField]
    private float _virtualScale = 1.0f;
    public float VirtualScale
    {
        get { return _virtualScale; }
        set { _virtualScale = value; }
    }

    /// <summary>
    /// setup root and dictionary
    /// </summary>
    public void Setup()
    {
        // if there's no child object, clear dictionary and null root
        if (this.gameObject.transform.childCount == 0)
        {
            Dict.Clear();
            Root = null;
            return;
        }
        else // construct internal data structure and dictionary if there's a loaded object
        {
            // initialize root
            Root = new Node(this.transform.GetChild(0).gameObject, null);

            // setup dictionary
            Dict.Clear();
            List<Node> curNodes = new List<Node>();
            curNodes.Add(Root);
            while (curNodes.Count > 0)
            {
                Node curNode = curNodes[0];
                Dict.Add(curNode.GameObject, curNode);
                foreach (var child in curNode.Childs)
                    curNodes.Add(child);
                curNodes.RemoveAt(0);
            }
        }
    }

	// Use this for initialization
	void Start () {
        Setup();
	}

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTransformChildrenChanged()
    {
        Setup();
    }

    public void FitToScale()
    {
    }

    public void OnDrawGizmos()
    {
        // draw gizmos for each node in the hierarchy
        if(Root != null)
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

            // call draw for each child
            foreach(var child in node.Childs)
            {
                DrawNode(child);
            }
        }
    }
}
