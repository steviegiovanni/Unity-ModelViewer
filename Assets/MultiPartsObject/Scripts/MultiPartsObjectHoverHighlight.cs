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

        /// <summary>
        /// materials used when highlighting an objerct
        /// </summary>
        [SerializeField]
        private Material _highlightMaterial;

        /// <summary>
        /// record the last gameobject that was previously hovered so we don't have to instantiate another highlight copy
        /// if we're hovering on the same object
        /// </summary>
        private GameObject previouslyHovered = null;
        private GameObject tempHighlight = null;

        /// <summary>
        /// coroutine to wait for an object pointer and register an onhoverevent listener
        /// </summary>
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

        /// <summary>
        /// triggered when object pointer is hovering on a gameobject
        /// </summary>
        void OnHoverEvent(GameObject hovered)
        {
            // if there's no hovered object, destroyed the temporary highlight object
            if(hovered == null)
            {
                previouslyHovered = null;
                if (tempHighlight != null)
                    GameObject.Destroy(tempHighlight);
            } else if (previouslyHovered != hovered) 
            {
                // only instantiate a new temporary highlight object if the current hovered object is different from the previous one
                // destroy the previous temporary highlight object first
                previouslyHovered = hovered;
                if (tempHighlight != null)
                    GameObject.Destroy(tempHighlight);
                tempHighlight = Instantiate(hovered,hovered.transform.position,hovered.transform.rotation);
                tempHighlight.transform.localScale = hovered.transform.lossyScale;
                // remove the collider of the highlight object and change the material to the highlight material
                if (tempHighlight.GetComponent<Collider>() != null)
                    GameObject.Destroy(tempHighlight.GetComponent<Collider>());
                if (tempHighlight.GetComponent<Renderer>() != null)
                    tempHighlight.GetComponent<Renderer>().material = _highlightMaterial;
            }
        }
    }
}
