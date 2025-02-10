using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSeedConfig", menuName = "Congklak/Seed Config")]
public class SeedConfig : ScriptableObject
{
    public GameObject defaultSeedPrefab; // Default biji congklak
    public List<GameObject> specialSeedPrefabs; // List prefab biji spesial
}
