using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryGridTile : MonoComponent {
    [SerializeField]
    private FactoryGridTileState tileState;

    public FactoryGridTileState TileState {
        get {
            return tileState;
        }

        set {
            tileState = value;
        }
    }
}
