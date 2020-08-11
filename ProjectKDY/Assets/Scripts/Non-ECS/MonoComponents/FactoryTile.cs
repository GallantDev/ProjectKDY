using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTile : MonoComponent {
    [SerializeField]
    private FactoryTileTypes tileType;

    public FactoryTileTypes TileType {
        get {
            return tileType;
        }

        set {
            tileType = value;
        }
    }
}
