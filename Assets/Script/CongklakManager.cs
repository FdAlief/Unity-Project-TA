using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongklakManager : MonoBehaviour
{
    public GameObject seedCongklakPrefab; // Prefab biji congklak
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
        // List biji yang sudah ditempatkan
        List<int> seedsPerHole = new List<int>(new int[holes.Length]);

        // Random distribution
        for (int i = 0; i < totalSeeds; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, holes.Length);

                // Abaikan elemen ke-4 (Hole Left)
            } while (seedsPerHole[randomIndex] >= maxSeedsPerHole || randomIndex == 4);

            // Tambahkan biji ke lubang
            seedsPerHole[randomIndex]++;
            PlaceSeedInHole(holes[randomIndex]);
        }
    }

    // Method untuk peletakkan posisi dan rotasi biji pada Holes pertama kalinya
    void PlaceSeedInHole(Transform hole)
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
        GameObject seed = Instantiate(seedCongklakPrefab, hole.position + offset, Quaternion.identity);

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
}