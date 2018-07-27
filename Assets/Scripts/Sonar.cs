using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    public class Sonar : MonoBehaviour
    {
        public MultiPartsObject mpo;
        public float cooldown = 5.0f;
        public float elapsed = 0.0f;
        public GameObject signal;

        public void Update()
        {
            if (mpo.Root == null) return;

            elapsed += Time.deltaTime;
            if (elapsed >= cooldown)
            {
                elapsed = 0.0f;

                Node foundNode = FindComponent();
                if((foundNode != null) && (signal != null))
                {
                    Instantiate(signal, foundNode.GameObject.transform.position, foundNode.GameObject.transform.rotation);
                }

            }
        }

        public Node FindComponent()
        {
            Node retval = null;
            List<Node> toCheck = new List<Node>();
            toCheck.Add(mpo.Root);
            bool found = false;
            while (toCheck.Count > 0 && !found)
            {
                Node curNode = toCheck[0];
                Vector3 oriPosAfterSetup = mpo.transform.TransformPoint(curNode.P0);
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
}
