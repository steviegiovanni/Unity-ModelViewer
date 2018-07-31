/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
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
        /// the starting bounds of the mesh on the node
        /// </summary>
        private Bounds _bounds;
        public Bounds Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        /// <summary>
        /// whether the node is selected or not
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// original material of the geometry in the node
        /// </summary>
        private Material _material;
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        /// <summary>
        /// whether the object is interactable or not
        /// </summary>
        private bool _locked;
        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; }
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
            Material = null;
            Locked = true;
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

            // get starting bounds
            if (HasMesh)
                Bounds = go.GetComponent<Renderer>().bounds;
            else
                Bounds = new Bounds(go.transform.position, Vector3.zero);

            // get original material
            if (HasMesh)
                Material = go.GetComponent<Renderer>().sharedMaterial;
            else
                Material = null;

            Locked = true;

            // add collider if doesn't exist
            if (HasMesh && go.GetComponent<Collider>() == null)
                go.AddComponent<MeshCollider>();

            // check childs
            foreach (Transform child in go.transform)
            {
                Childs.Add(new Node(child.gameObject, this));
            }
        }

		/// <summary>
		/// constructor that takes a go and a parent node as well as the cage transform
		/// </summary>
		public Node(GameObject go, Node parent, Transform cage)
		{
			// assign parent
			Parent = parent;

			// assign game object
			GameObject = go;

			// get the original transforms, position relative to the cage
			P0 = cage.InverseTransformPoint(go.transform.position);
			R0 = go.transform.rotation;
			S0 = go.transform.localScale;

			// check whether game object has a mesh
			HasMesh = go.GetComponent<MeshFilter>() != null;

			// get starting bounds
			if (HasMesh)
				Bounds = go.GetComponent<Renderer>().bounds;
			else
				Bounds = new Bounds(go.transform.position, Vector3.zero);

			// get original material
			if (HasMesh)
				Material = go.GetComponent<Renderer>().sharedMaterial;
			else
				Material = null;

            Locked = true;

			// add collider if doesn't exist
			if (HasMesh && go.GetComponent<Collider>() == null)
				go.AddComponent<MeshCollider>();

			// check childs
			foreach (Transform child in go.transform)
			{
				Childs.Add(new Node(child.gameObject, this,cage));
			}
		}

        public Node(SerializableNode sn)
        {
            //Childs = children,
            GameObject = sn.GameObject;
            HasMesh = sn.HasMesh;
            //Parent = null,
            P0 = sn.P0;
            R0 = sn.R0;
            S0 = sn.S0;
            Bounds = sn.Bounds;
            Material = sn.Material;
            Locked = sn.Locked;
        }

        /// <summary>
        /// return the cumulative bounds of a node and its childs
        /// </summary>
        public Bounds GetCumulativeBounds()
        {
            if (Childs.Count == 0)
                return Bounds;
            else
            {
                Bounds b = Bounds;
                foreach (var child in Childs)
                    b.Encapsulate(child.GetCumulativeBounds());
                return b;
            }
        }
    }

    /// <summary>
    /// structure used to store node data after serialization
    /// </summary>
    [System.Serializable]
    public struct SerializableNode
    {
        public int ChildCount;
        public int IndexOfFirstChild;
        public GameObject GameObject;
        public bool HasMesh;
        public int indexOfParent;
        public Vector3 P0;
        public Quaternion R0;
        public Vector3 S0;
        public Bounds Bounds;
        public Material Material;
        public bool Locked;
    }

    /// <summary>
    /// a multi parts object is an object that consists of several different parts
    /// that can be detached and assembled back again
    /// </summary>
    public class MultiPartsObject : MonoBehaviour, ISerializationCallbackReceiver
    {
		/// <summary>
		/// original position of the cage
		/// </summary>
		private Vector3 _cagePos;
		public Vector3 CagePos{
			get{ return _cagePos;}
			set{ _cagePos = value;}
		}

        /// <summary>
        /// the root node
        /// </summary>
        private Node _root;
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
            get
            {
                if (_dict == null)
                    _dict = new Dictionary<GameObject, Node>();
                return _dict;
            }
        }

        /// <summary>
        /// the virtual size of the object
        /// </summary>
        [Range(0.0f, 10.0f)]
        [SerializeField]
        private float _virtualScale = 1.0f;
        public float VirtualScale
        {
            get { return _virtualScale; }
            set { _virtualScale = value; }
        }

        /// <summary>
        /// output current scale of object to fit the virtual scale
        /// </summary>
        [SerializeField]
        private float _currentScale;
        public float CurrentScale
        {
            get { return _currentScale; }
            private set { _currentScale = value; }
        }

        /// <summary>
        /// list of currently selected nodes
        /// </summary>
        private List<Node> _selectedNodes;
        public List<Node> SelectedNodes
        {
            get
            {
                if (_selectedNodes == null)
                    _selectedNodes = new List<Node>();
                return _selectedNodes;
            }
        }

        /// <summary>
        /// highlight material for selected nodes
        /// </summary>
        [SerializeField]
        private Material _highlightMaterial;
        public Material HighlightMaterial
        {
            get { return _highlightMaterial; }
            set { _highlightMaterial = value; }
        }

		/// <summary>
		/// highlight material for selected nodes
		/// </summary>
		[SerializeField]
		private Material _silhouetteMaterial;
		public Material SilhouetteMaterial
		{
			get { return _silhouetteMaterial; }
			set { _silhouetteMaterial = value; }
		}

        /// <summary>
        /// the frame attached to the controller frame (could be the head for gazing)
		/// this frame will be used to contain all the selected nodes when grabbed
        /// </summary>
        [SerializeField]
        private GameObject _movableFrame;
        public GameObject MovableFrame
        {
            get { return _movableFrame; }
        }
			

		/// <summary>
		/// The snap threshold
		/// </summary>
		[Range(0.0f, 1.0f)]
		[SerializeField]
		private float _snapThreshold = 0.1f;
		public float SnapThreshold{
			get{ return _snapThreshold;}
			set{ _snapThreshold = value;}
		}

		/// <summary>
		/// indicate whether to deselect selected node if they snap
		/// </summary>
		[SerializeField]
		private bool _deselectOnSnapped = true;
		public bool DeselectOnSnapped{
			get{ return _deselectOnSnapped;}
			set{ _deselectOnSnapped = value;}
		}

		/// <summary>
		/// container for temporary runtime instantiated silhouette object
		/// </summary>
		private GameObject _silhouette = null;

        // Use this for initialization
        void Start()
        {
			// get the initial position of the cage. on runtime the cage will be moved around to make the focused object centered
			CagePos = this.transform.position;

			// check movable frame exists
            if (MovableFrame == null)
                Debug.LogWarning("no movable frame assigned. will not be able to move objects around.");

            /*Setup();
            FitToScale(Root, VirtualScale);
			SetupSilhouette ();
			Scatter (Root);*/
        }

		/// <summary>
		/// Setups the silhouette (hints for where all the parts should be placed)
		/// </summary>
		public void SetupSilhouette(){
			// destry previous silhouette
			if (_silhouette != null)
				Destroy (_silhouette);

			// create temporary go
			_silhouette = new GameObject ("Silhouette");

			// sync transform with cage's transform
			_silhouette.transform.SetPositionAndRotation (this.transform.position, this.transform.rotation);
			_silhouette.transform.localScale = this.transform.localScale;

			// if setup (root is not null), copy the model
			if (Root != null) {
				GameObject silhouette = Instantiate (Root.GameObject, _silhouette.transform);
				MakeSilhouette (silhouette); // call makesilhouette recursive
			}
		}

		/// <summary>
		/// recursive function to create a silhouette of the viewed model
		/// </summary>
		public void MakeSilhouette (GameObject go){
			if (go.GetComponent<Collider> () != null)
				Destroy (go.GetComponent<Collider> ());
			if (go.GetComponent<Renderer> () != null)
				go.GetComponent<Renderer> ().material = SilhouetteMaterial;
			foreach (Transform child in go.transform)
				MakeSilhouette (child.gameObject);
		}

		/// <summary>
		/// recursive function to scatter components of viewed model around the area
		/// </summary>
		public void Scatter(Node node){
			if (node.HasMesh) {
				node.GameObject.transform.position = Random.insideUnitSphere * 2;
			}

			foreach (var child in node.Childs)
				Scatter (child);
		}

        // Update is called once per frame
        void Update()
        {
            // test input without AR/VR setup
            if (Input.GetKeyUp(KeyCode.Q))
            {
                Grab();
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                Release();
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                Select();
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                Deselect();
            }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                if (Root != null)
                    Debug.Log(Root.GameObject.name);
            }
        }

        /// <summary>
        /// setup root and dictionary
        /// </summary>
        public void Setup()
        {
			// clear selected nodes
            SelectedNodes.Clear();

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
				Root = new Node(this.transform.GetChild(0).gameObject, null,this.transform);
            }
        }

        /// <summary>
        /// construct dictionary for easy mapping between a gameobject and a node
        /// </summary>
        public void ConstructDictionary()
        {
            Dict.Clear();
            if (Root == null) return;
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

        /// <summary>
        /// resize the cage to fit the focused node to a scale
        /// </summary>
        public void FitToScale(Node node, float scale)
        {
            if (node != null)
            {
                Bounds b = node.GetCumulativeBounds();
                float scaleFactor = scale / b.size.magnitude;
                CurrentScale = scaleFactor;
				this.transform.localScale = Vector3.one * scaleFactor;
				//this.transform.position = this.transform.position - (b.center  - this.transform.position) * scaleFactor;
				this.transform.position = CagePos - (b.center  - CagePos) * scaleFactor;
                //node.GameObject.transform.localScale = node.S0 * scaleFactor;
				//node.GameObject.transform.position = this.transform.position - (b.center - this.transform.position) * scaleFactor;
            }
        }

        /// <summary>
        /// reset transform of a node and its children recursively
        /// </summary>
        public void ResetAll(Node node)
        {
            CurrentScale = 1.0f;
            if (node != null)
            {
				node.GameObject.transform.SetPositionAndRotation(this.transform.TransformPoint(node.P0), node.R0);
                node.GameObject.transform.localScale = node.S0;
                foreach (var child in node.Childs)
                    ResetAll(child);
            }
        }

        /// <summary>
        /// select a node that contains the game object pointed by the objectpointer
        /// </summary>
        public void Select()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
                Select(ObjectPointer.Instance.HitInfo.collider.gameObject);
        }

        /// <summary>
        /// select a node that contains the specified game object
        /// </summary>
        public void Select(GameObject go)
        {
            Node selectedNode = null;
            if (Dict.TryGetValue(go, out selectedNode))
                Select(selectedNode);
        }

        /// <summary>
        /// select the specified node
        /// </summary>
        public void Select(Node node)
        {
            if (node.Locked) return;
            node.Selected = true;
            if (node.HasMesh)
                node.GameObject.GetComponent<Renderer>().material = new Material(HighlightMaterial);
            //if (!SelectedNodes.Contains(node))
            SelectedNodes.Add(node);
        }

        /// <summary>
        /// deselect a node that contains the game object pointed by the objectpointer
        /// </summary>
        public void Deselect()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
                Deselect(ObjectPointer.Instance.HitInfo.collider.gameObject);
        }

        /// <summary>
        /// deselect a node that contains the specified game object
        /// </summary>
        public void Deselect(GameObject go)
        {
            Node deselectedNode = null;
            if (Dict.TryGetValue(go, out deselectedNode))
                Deselect(deselectedNode);
        }

        /// <summary>
        /// Deselect the specified node.
        /// </summary>
        public void Deselect(Node node)
        {
            if (node.Locked) return;
            node.Selected = false;
            if (node.HasMesh)
            {
                node.GameObject.GetComponent<Renderer>().material = node.Material;
            }
            SelectedNodes.Remove(node);
        }

        /// <summary>
        /// toggle select a node that contains the game object pointed by the objectpointer
        /// </summary>
        public void ToggleSelect()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
                ToggleSelect(ObjectPointer.Instance.HitInfo.collider.gameObject);
        }

        /// <summary>
        /// Toggle select a node that contains this game object
        /// </summary>
        public void ToggleSelect(GameObject go)
        {
            Node node = null;
            if (Dict.TryGetValue(go, out node))
                ToggleSelect(node);
        }

        /// <summary>
        /// Toggles select the specified node
        /// </summary>
        public void ToggleSelect(Node node)
        {
            if (node.Selected)
                Deselect(node);
            else
                Select(node);
        }

        /// <summary>
        /// grab all selected object and put them under movable frame
        /// </summary>
        public void Grab()
        {
            if (MovableFrame != null)
            {
                foreach (var obj in SelectedNodes)
                {
                    if (obj.Childs.Count > 0)
                    {
                        foreach (var child in obj.Childs)
                            if (child.GameObject.transform.parent.gameObject == child.Parent.GameObject)
                                child.GameObject.transform.SetParent(this.transform);
                    }
                    obj.GameObject.transform.SetParent(MovableFrame.transform);
                }
            }
        }

        /// <summary>
        /// call grab if pointer is pointing at a gameobject that is one of the selected node
        /// </summary>
        public void GrabIfPointingAt()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
            {
                GameObject hitObject = ObjectPointer.Instance.HitInfo.collider.gameObject;
                Node hitNode = null;
                if (Dict.TryGetValue(hitObject, out hitNode))
                {
					if (SelectedNodes.Contains (hitNode)) {
						MovableFrame.transform.position = ObjectPointer.Instance.HitInfo.point;
						Grab ();
					}
                }
            }
        }

        /// <summary>
        /// release all selected object and put them back to their original structure
        /// </summary>
        public void Release()
        {
			Node[] selectedArray = SelectedNodes.ToArray ();
			for (int i = 0; i < selectedArray.Length; i++) {
				if (selectedArray [i].Childs.Count > 0) {
					foreach (var child in selectedArray[i].Childs)
						child.GameObject.transform.SetParent (selectedArray [i].GameObject.transform);
				}

				if (selectedArray [i].Parent == null)
					selectedArray [i].GameObject.transform.SetParent (this.transform);
				else
					selectedArray [i].GameObject.transform.SetParent (selectedArray [i].Parent.GameObject.transform);

				Snap (selectedArray [i]);
			}
        }

		/// <summary>
		/// Snap the specified node to its original position in the cage if it's close enough
		/// </summary>
		public void Snap(Node node){
			//Vector3 oriPosRelativeToCage = node.P0 - CagePos;
			Vector3 oriPosAfterSetup = this.transform.TransformPoint(node.P0);
			if (Vector3.Distance (node.GameObject.transform.position,oriPosAfterSetup) < SnapThreshold) {
				node.GameObject.transform.position = oriPosAfterSetup;
				node.GameObject.transform.rotation = node.R0;

				if (DeselectOnSnapped)
					Deselect (node);
			}
		}


        /// ==================================================
        /// Gizmo stuff
        /// ==================================================
        public void OnDrawGizmos()
        {
            if (Root != null)
            {
                // draw gizmos for each node in the hierarchy
                DrawNode(Root);

                // draw root cumulative bounding box
                /*Bounds b = Root.GetCumulativeBounds();

                // draw bounding sphere
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(Vector3.zero,b.extents.magnitude * CurrentScale);*/
            }
        }

        public void DrawNode(Node node)
        {
            if (node != null)
            {
                // draw origin
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(node.GameObject.transform.position, 0.01f);

                // draw bounding box if any
                if (node.HasMesh && node.Selected)
                {
                    Gizmos.color = Color.green;
                    Renderer rend = node.GameObject.GetComponent<Renderer>();
                    Mesh mesh = node.GameObject.GetComponent<MeshFilter>().sharedMesh;
                    Bounds bounds = rend.bounds;
                    //Bounds bounds = mesh.bounds;
                    Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
                    //Gizmos.DrawWireSphere(rend.bounds.center,rend.bounds.extents.magnitude);
                }

                // call draw for each child
                foreach (var child in node.Childs)
                {
                    DrawNode(child);
                }
            }
        }

        /// ==================================================
        /// Serializing the node structure
        /// ==================================================

        /// <summary>
        /// the serialized node structure
        /// </summary>
        public List<SerializableNode> serializedNodes;

        /// <summary>
        /// add a serialized node into the list of serialized nodes
        /// </summary>
        void AddNodeToSerializedNodes(Node n, int parentId)
        {
            if (n == null) return;

            var serializedNode = new SerializableNode()
            {
                ChildCount = n.Childs.Count,
                IndexOfFirstChild = serializedNodes.Count + 1,
                GameObject = n.GameObject,
                HasMesh = n.HasMesh,
                indexOfParent = parentId,
                P0 = n.P0,
                R0 = n.R0,
                S0 = n.S0,
                Bounds = n.Bounds,
                Material = n.Material,
                Locked = n.Locked
            };

            serializedNodes.Add(serializedNode);
            foreach (var child in n.Childs)
                AddNodeToSerializedNodes(child,serializedNode.IndexOfFirstChild - 1);
        }

        /// <summary>
        /// serialization interface implementation
        /// </summary>
        public void OnBeforeSerialize()
        {
            serializedNodes.Clear();
            AddNodeToSerializedNodes(Root,-1);
        }

        /// <summary>
        /// create a new node from a serialized node index
        /// </summary>
        Node ReadNodeFromSerializedNodes(int index, Node parent)
        {
            if (index < 0)
                return null;

            var serializedNode = serializedNodes[index];
            Node node = new Node(serializedNode);
            node.Parent = parent;
            var children = new List<Node>();
            for (int i = 0; i != serializedNode.ChildCount; i++)
                children.Add(ReadNodeFromSerializedNodes(serializedNode.IndexOfFirstChild + i,node));
            node.Childs = children;
            return node;
        }

        /// <summary>
        /// deserialization interface implementation
        /// </summary>
        public void OnAfterDeserialize()
        {
            if (serializedNodes.Count > 0)
            {
                Root = ReadNodeFromSerializedNodes(0, null);
                ConstructDictionary();
            }
            else
                Root = null;
        }
    }
}