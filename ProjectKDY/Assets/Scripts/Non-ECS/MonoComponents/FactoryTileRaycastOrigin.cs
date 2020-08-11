using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTileRaycastOrigin : MonoComponent {
    [SerializeField]
    private Vector3 localOriginPoint = Vector3.zero;

    public Vector3 LocalOriginPoint {
        get {
            return localOriginPoint;
        }

        set {
            localOriginPoint = value;
        }
    }
}