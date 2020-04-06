using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    [SerializeField]
    private int mapLength = 0;
    [SerializeField]
    private int mapWidth = 0;
    [SerializeField]
    private GameObject mapTilePrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateMap() {
        Vector3 originPoint = Vector3.zero;
        Vector3 spawnPoint;
        float halfLength = mapLength / 2.0f;
        float halfWidth = mapWidth / 2.0f;
        for (int x = 0; x < mapLength; x++) {
            for (int y = 0; y < mapWidth; y++) {
                spawnPoint = new Vector3((-mapLength + x), 0.0f, (-mapWidth + y));
                Instantiate(mapTilePrefab, spawnPoint, Quaternion.identity);
            }
        }
    }
}