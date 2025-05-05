using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSeedConfig", menuName = "Congklak/Seed Config")]
public class SeedConfig : ScriptableObject
{
    public GameObject defaultSeedPrefab; // Default biji congklak (Digunakan pada Script CongklakManager)

    public List<GameObject> specialSeedPrefabs; // List prefab biji spesial (Digunakan pada Script CongklakManager)

    public List<SeedSpecialData> seedStoreList; // List data Seed untuk Store (Digunakan pada Script StoreManager)

    // Helper untuk cari data spesial seed berdasarkan prefab name
    // Digunakan pada Script InventoryManager (OpenPanelDeleteSpecialSeed)
    public SeedSpecialData GetSeedDataByPrefabName(string prefabName)
    {
        string cleanName = prefabName.Replace("(Clone)", "").Trim();
        return seedStoreList.Find(seed => seed.seedPrefab != null && seed.seedPrefab.name == cleanName);
    }
}

// Script Class untuk menampung data Per Jenis Biji Spesial
// Digunakan pada List Data seedStoreList
[System.Serializable]
public class SeedSpecialData
{
    public string seedName;
    [TextArea] public string seedInfo;
    public Sprite seedImage;
    public GameObject seedPrefab;
    public int price;
}
