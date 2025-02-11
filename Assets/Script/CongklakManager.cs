using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongklakManager : MonoBehaviour
{
    [Header("Biji")]
    public SeedConfig seedConfig; // Menggunakan ScriptableObject untuk seed

    [Header("Hole")]
    public Transform[] holes; // Array untuk lubang congklak

    private int totalSeeds = 56; // Total biji congklak
    private int maxSeedsPerHole = 7; // Maksimal biji per lubang

    void Start()
    {
        DistributeSeeds();
    }

    // Method untuk men-distribusi / memasukkan biji ke Holes untuk pertama kalinya secara random
    void DistributeSeeds()
    {
        List<int> seedsPerHole = new List<int>(new int[holes.Length]); // Menyimpan jumlah biji per hole
        List<GameObject> specialSeeds = new List<GameObject>(seedConfig.specialSeedPrefabs); // Copy daftar biji spesial

        // Step 1: Masukkan biji spesial (hanya 1 per jenis)
        foreach (GameObject specialSeed in specialSeeds)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, holes.Length);
            } while (seedsPerHole[randomIndex] >= maxSeedsPerHole || randomIndex == 4);

            seedsPerHole[randomIndex]++;
            PlaceSeedInHole(holes[randomIndex], specialSeed);
        }

        // Step 2: Masukkan sisa biji menggunakan default seedPrefab
        int remainingSeeds = totalSeeds - specialSeeds.Count;
        for (int i = 0; i < remainingSeeds; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, holes.Length);
            } while (seedsPerHole[randomIndex] >= maxSeedsPerHole || randomIndex == 4);

            seedsPerHole[randomIndex]++;
            PlaceSeedInHole(holes[randomIndex], seedConfig.defaultSeedPrefab);
        }
    }

    // Method untuk peletakkan posisi dan rotasi biji pada Holes pertama kalinya
    void PlaceSeedInHole(Transform hole, GameObject seedPrefab)
    {
        // Tentukan radius untuk distribusi biji di sekitar pusat lubang
        float radius = 0.25f;

        // Hitung sudut acak untuk penempatan biji
        float angle = Random.Range(0f, 360f);
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
            0
        );

        // Buat instance biji congklak dengan posisi offset
        GameObject seed = Instantiate(seedPrefab, hole.position + offset, Quaternion.identity);

        // Atur rotasi acak agar terlihat lebih alami
        seed.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // Atur scale biji untuk pertama kali
        seed.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

        // Set prefab sebagai child dari lubang
        seed.transform.SetParent(hole);

        // Menggunakan method pada script Congklak Hole
        // Tambahkan biji ke dalam list seedsInHole pada lubang
        CongklakHole holeScript = hole.GetComponent<CongklakHole>();
        if (holeScript != null)
        {
            holeScript.AddSeed(seed);  // Menambahkan biji ke lubang yang sesuai
        }
    }

    // Method untuk mereset semua biji di setiap hole
    // Untuk ketika Selesai Stage akan mereset biji pada congklak
    // Digunakan pada script StageManager & WinScript
    public void ResetSeeds()
    {
        // Hapus semua biji dari setiap hole
        foreach (Transform hole in holes)
        {
            CongklakHole holeScript = hole.GetComponent<CongklakHole>();
            if (holeScript != null)
            {
                holeScript.ClearSeeds(); // Hapus semua biji di lubang
            }
        }

        // Redistribusi biji
        DistributeSeeds();
    }
}