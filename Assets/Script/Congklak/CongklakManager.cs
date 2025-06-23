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

    // Method untuk meletakkan biji pada lubang congklak secara acak dan memberikan efek muncul dari atas
    // Digunakan pada Script CongklakHole (BaliSpecialSeed)
    public void PlaceSeedInHole(Transform hole, GameObject seedPrefab)
    {
        float radius = 0.25f; // Radius jarak dari titik pusat lubang untuk posisi awal biji
        float angle = Random.Range(0f, 360f); // Sudut acak untuk menentukan posisi biji di sekitar lubang

        // Hitung offset posisi berdasarkan sudut dan radius, untuk menyebar biji di sekitar lubang
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
            0
        );

        Vector3 targetPosition = hole.position + offset; // Posisi akhir biji di lubang

        // Posisi awal biji, muncul dari atas secara acak
        Vector3 startPosition = targetPosition + new Vector3(Random.Range(-1f, 1f), 1.5f, 0f);

        // Instantiate prefab biji di posisi awal dengan rotasi default
        GameObject seed = Instantiate(seedPrefab, startPosition, Quaternion.identity);
        seed.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f); // Skala kecil untuk biji

        // Mulai animasi perpindahan biji ke lubang
        StartCoroutine(AnimateSeedToHole(seed, targetPosition, hole));
    }

    // Coroutine untuk menganimasikan perpindahan dan rotasi biji menuju lubang secara halus
    private IEnumerator AnimateSeedToHole(GameObject seed, Vector3 targetPosition, Transform parentHole)
    {
        float duration = 0.25f; // Durasi animasi
        float elapsed = 0f; // Waktu yang telah berlalu selama animasi
        Vector3 startPosition = seed.transform.position; // Posisi awal animasi
        Quaternion startRotation = seed.transform.rotation; // Rotasi awal biji
        Quaternion targetRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f)); // Rotasi acak sebagai efek visual

        // Loop animasi
        while (elapsed < duration)
        {
            float t = elapsed / duration; // Nilai interpolasi 0–1
            seed.transform.position = Vector3.Lerp(startPosition, targetPosition, t); // Interpolasi posisi
            seed.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t); // Interpolasi rotasi
            elapsed += Time.deltaTime; // Tambahkan waktu frame ini
            yield return null; // Tunggu frame berikutnya
        }

        // Set posisi dan rotasi akhir
        seed.transform.position = targetPosition;
        seed.transform.rotation = targetRotation;
        seed.transform.SetParent(parentHole); // Parent biji ke lubang (untuk organisasi di hierarchy)

        // Tambahkan biji ke skrip lubang, jika ada
        CongklakHole holeScript = parentHole.GetComponent<CongklakHole>();
        if (holeScript != null)
        {
            holeScript.AddSeed(seed, false);
        }
    }

    // Method untuk mereset semua biji di setiap hole
    // Digunakan pada script WinScript, LoseScript, StageManager, StageInput
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

    // Method untuk mengahpus semua biji di setiap hole
    // Digunakan pada script StageManager dan Button Back Stage Menu
    public void ClearAllSeeds()
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
    }

    // Method untuk mengatur Aktif/Non-Aktif Seed pada Hole
    // Digunakan pada Button Pause dan Resume Pause
    public void SetActiveAllSeeds(bool isActive)
    {
        // Hapus semua biji dari setiap hole
        foreach (Transform hole in holes)
        {
            CongklakHole holeScript = hole.GetComponent<CongklakHole>();
            if (holeScript != null)
            {
                holeScript.SetActiveSeeds(isActive);
            }
        }
    }
}