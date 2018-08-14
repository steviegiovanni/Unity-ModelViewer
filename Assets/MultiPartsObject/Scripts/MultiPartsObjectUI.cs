using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// simple UI, listens to on hover node event and shows the part name in front of the user 
    /// </summary>
    public class MultiPartsObjectUI : MonoBehaviour
    {
        [SerializeField]
        private TextMesh _partName;
        public TextMesh PartName
        {
            get { return _partName; }
            set { _partName = value; }
        }

        [SerializeField]
        private MultiPartsObject _mpo;

        // Use this for initialization
        void Start()
        {
            ObjectPointer.Instance.OnHoverEvent.AddListener(OnHoverEvent);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnHoverEvent(GameObject hovered)
        {
            if (hovered == null)
            {
                if (PartName != null)
                    PartName.gameObject.SetActive(false);
            }
            else
            {
                if(PartName != null &&  _mpo != null)
                {
                    Node nodeInfo = null;
                    if(_mpo.Dict.TryGetValue(hovered,out nodeInfo)) {
                        PartName.text = nodeInfo.Name;
                        PartName.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
