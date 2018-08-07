using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HololensRayObject : MonoBehaviour, IHasRay {
    public GazeStabilizer raySource;

    public Ray GetRay()
    {
        return raySource.StableRay;
    }
}
