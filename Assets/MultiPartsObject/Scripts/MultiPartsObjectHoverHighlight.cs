// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ModelViewer
{
    /// <summary>
    /// a class that listens to on hover node event and highlight the corresponding part
    /// </summary>
    public class MultiPartsObjectHoverHighlight : MonoBehaviour
    {
        /// <summary>
        /// the multipartsobject we're going to query this object to get the node info once we get a hover event
        /// </summary>
        [SerializeField]
        private MultiPartsObject _mpo;

        [SerializeField]
        private Material _highlightMaterial;

        private GameObject previouslyHovered = null;
        private GameObject tempHighlight = null;

        public IEnumerator ListensToObjectPointerHover()
        {
            while (ObjectPointer.Instance == null)
                yield return null;
            ObjectPointer.Instance.OnHoverEvent.AddListener(OnHoverEvent);
            yield return null;
        }

        // Use this for initialization
        void Start()
        {
            StartCoroutine(ListensToObjectPointerHover());
        }

        // Update is called once per frame
        void Update()
        {
            if(previouslyHovered != null && tempHighlight != null)
            {
                tempHighlight.transform.SetPositionAndRotation(previouslyHovered.transform.position, previouslyHovered.transform.rotation);
            }
        }

        void OnHoverEvent(GameObject hovered)
        {
            if(hovered == null)
            {
                previouslyHovered = null;
                if (tempHighlight != null)
                    GameObject.Destroy(tempHighlight);
            } else if (previouslyHovered != hovered)
            {
                previouslyHovered = hovered;
                if (tempHighlight != null)
                    GameObject.Destroy(tempHighlight);
                tempHighlight = Instantiate(hovered,hovered.transform.position,hovered.transform.rotation);
                tempHighlight.transform.localScale = hovered.transform.lossyScale;
                if (tempHighlight.GetComponent<Collider>() != null)
                    GameObject.Destroy(tempHighlight.GetComponent<Collider>());
                if (tempHighlight.GetComponent<Renderer>() != null)
                    tempHighlight.GetComponent<Renderer>().material = _highlightMaterial;
            }
        }
    }
}
