using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDataManager : MonoBehaviour
{
    public Terrain terrain;             // 分離したいTerrain
    public TerrainData newTerrainData;  // 複製したTerrainData

    void Start()
    {
        terrain.terrainData = newTerrainData;
    }
}
