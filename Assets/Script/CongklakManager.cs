using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongklakManager : MonoBehaviour
{
    public GameObject congklakPrefab; // Prefab biji congklak
    public Transform[] holes; // Array untuk lubang congklak

    private int totalSeeds = 56; // Total biji congklak
    private int maxSeedsPerHole = 7; // Maksimal biji per lubang

    void Start()
    {
        DistributeSeeds();
    }

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
            }
            while (seedsPerHole[randomIndex] >= maxSeedsPerHole);

            // Tambahkan biji ke lubang
            seedsPerHole[randomIndex]++;
            PlaceSeedInHole(holes[randomIndex]);
        }
    }

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
        GameObject seed = Instantiate(congklakPrefab, hole.position + offset, Quaternion.identity);

        // Atur rotasi acak agar terlihat lebih alami
        seed.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // Set prefab sebagai child dari lubang
        seed.transform.SetParent(hole);
    }
}
