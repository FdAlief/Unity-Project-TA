using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSeedHandler : MonoBehaviour
{
    [Header("Data Biji")]
    [SerializeField] private SeedConfig seedConfig; // Data biji default dan spesial

    [HideInInspector] public bool isBorobudurSeedUsed = false; // Flag untuk biji Borobudur ketika digunakan

    [Header("Referensi Script")]   
    [SerializeField] private CongklakManager congklakManager; // Referensi ke congklak manager
    [SerializeField] private ColliderHoleManager colliderHole; // Referensi ke collider hole manager
    [SerializeField] private InventoryManager inventoryManager; // Referensi ke inventory manager

    // Method untuk mengecek Biji Spesial pada Congklak Hole
    // Digunakan pada Script CongklakHole (AddSeed)
    public void HandleSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        MonasSpecialSeed(seed, congklakHole);
        BorobudurSpecialSeed(seed, congklakHole);
        KomodoSpecialSeed(seed, congklakHole);
        BaliSpecialSeed(seed, congklakHole);
        JamGadangSpecialSeed(seed, congklakHole);
        PatungSurabayaSpecialSeed(seed, congklakHole);
    }

    // Method untuk Biji Spesial (Monas)
    // Berfungsi melipat gandakan biji hanya pada Hole biasa, tidak pada Hole Besar dan bukan Biji terahir
    // Digunakan pada Method HandleSpecialSeed()
    private void MonasSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        // Cek apakah seed adalah seed special dan bukan pada hole sumber skor dan bukan biji terakhir
        bool isSpecial = seed.CompareTag("Monumen Nasional Seed");
        bool isLastSeed = inventoryManager.IsLastSeed(seed);

        if (isSpecial && !congklakHole.isScoreSource && !isLastSeed)
        {
            Debug.Log("Monas Seed Special terdeteksi dan akan menggandakan biji di Hole biasa");

            int jumlahAsli = congklakHole.seedsInHole.Count - 1; // Kurangi 1 karena seed special baru saja ditambahkan

            for (int i = 0; i < jumlahAsli; i++)
            {
                GameObject originalSeed = congklakHole.seedsInHole[i];
                GameObject duplicatedSeed = Instantiate(originalSeed, congklakHole.transform.position, Quaternion.identity, congklakHole.transform);
                congklakHole.seedsInHole.Add(duplicatedSeed);
            }

            congklakHole.UpdateSeedCountUI(); // Update UI setelah menggandakan biji
        }
    }

    // Method untuk Biji Spesial (Borobudur)
    // Berfungsi melipat gandakan reward coin yang di dapatkan ketika diletakkan pada Hole Besar dan Objective Complete
    // Digunakan pada Method HandleSpecialSeed() & Script StageManager (OnObjectiveComplete)
    private void BorobudurSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        bool isSpecial = seed.CompareTag("Borobudur Seed");

        if (isSpecial && congklakHole.isScoreSource)
        {
            Debug.Log("Borobudur Seed diletakkan di Hole Besar. Reward akan digandakan di akhir.");

            isBorobudurSeedUsed = true;
        }
    }

    // Method untuk Biji Spesial (Komodo)
    // Berfungsi melipat gandakan biji hanya pada Hole Besar
    // Digunakan pada Method HandleSpecialSeed()
    private void KomodoSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        // Cek apakah seed adalah seed special dan berada pada hole sumber skor (Hole Besar)
        bool isSpecial = seed.CompareTag("Komodo Seed");

        if (isSpecial && congklakHole.isScoreSource)
        {
            Debug.Log("Komodo Seed Special terdeteksi dan akan menggandakan biji di Hole Besar");

            int jumlahAsli = congklakHole.seedsInHole.Count - 1; // Kurangi 1 karena seed special baru saja ditambahkan

            for (int i = 0; i < jumlahAsli; i++)
            {
                GameObject originalSeed = congklakHole.seedsInHole[i];
                GameObject duplicatedSeed = Instantiate(originalSeed, congklakHole.transform.position, Quaternion.identity, congklakHole.transform);
                congklakHole.seedsInHole.Add(duplicatedSeed);
            }

            congklakHole.UpdateSeedCountUI(); // Update UI setelah menggandakan biji
        }
    }

    // Method untuk Biji Spesial (Bali)
    // Berfungsi menambahkan biji random (1-10) hanya pada Hole Besar
    // Digunakan pada Method HandleSpecialSeed()
    private void BaliSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        // Cek apakah seed adalah seed special dan berada pada hole sumber skor (Hole Besar)
        bool isSpecial = seed.CompareTag("Bali Seed");

        if (isSpecial && congklakHole.isScoreSource)
        {
            Debug.Log("Bali Seed terdeteksi. Menambahkan biji random ke Hole.");

            int jumlahRandom = Random.Range(1, 11); // 1 hingga 10 biji baru (karena max exclusive)

            for (int i = 0; i < jumlahRandom; i++)
            {
                // Melakukan penambahan dan peletakkan biji default pada Hole Besar
                congklakManager.PlaceSeedInHole(congklakHole.transform, seedConfig.defaultSeedPrefab);
            }

            congklakHole.UpdateSeedCountUI(); // Update UI setelah menambahkan biji
        }
    }

    // Method untuk Biji Spesial (Jam Gadang)
    // Berfungsi melipat gandakan biji pada Hole Berlawanan, tidak pada Hole Besar dan bukan Biji terahir
    // Digunakan pada Method HandleSpecialSeed()
    private void JamGadangSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        // Cek apakah seed adalah seed spesial dan bukan pada hole besar serta bukan biji terakhir
        bool isSpecial = seed.CompareTag("Jam Gadang Seed");
        bool isLastSeed = inventoryManager.IsLastSeed(seed);

        if (isSpecial && !congklakHole.isScoreSource && !isLastSeed)
        {
            Debug.Log("Jam Gadang Seed Spesial berlawanan terdeteksi. Menggandakan biji di hole lawan.");

            // Cari index hole saat ini
            int currentIndex = colliderHole.colliders.IndexOf(congklakHole.GetComponent<Collider>());
            if (currentIndex == -1)
            {
                Debug.LogError("Hole saat ini tidak ditemukan dalam daftar colliders.");
                return;
            }

            // Cek apakah ada mapping hole berlawanan
            if (colliderHole.oppositeHoles.TryGetValue(currentIndex + 1, out int oppositeIndex))
            {
                // Ambil hole berlawanan
                Collider oppositeCollider = colliderHole.colliders[oppositeIndex - 1];
                CongklakHole oppositeHole = oppositeCollider.GetComponent<CongklakHole>();

                if (oppositeHole != null)
                {
                    int jumlahAsli = oppositeHole.seedsInHole.Count;

                    for (int i = 0; i < jumlahAsli; i++)
                    {
                        GameObject originalSeed = oppositeHole.seedsInHole[i];
                        GameObject duplicatedSeed = Instantiate(originalSeed, oppositeHole.transform.position, Quaternion.identity, oppositeHole.transform);
                        oppositeHole.seedsInHole.Add(duplicatedSeed);
                    }

                    oppositeHole.UpdateSeedCountUI(); // Update UI di hole berlawanan
                    Debug.Log($"Berhasil menggandakan {jumlahAsli} biji di hole berlawanan: {oppositeHole.name}");
                }
            }
            else
            {
                Debug.LogWarning("Tidak ada mapping hole berlawanan untuk hole ini.");
            }
        }
    }

    // Method untuk Biji Spesial (Honai)
    // Berfungsi meng-aktifkan Hole Setelah dan Sebelum dari peletakkan terakhir, tidak pada Hole Besar (dilewati) dan Hole Kosong
    // Digunakan pada Script DragHandler (GetNouseButtonUp)
    public void HonaiSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        bool isSpecial = seed.CompareTag("Honai Seed");

        if (isSpecial)
        {
            Debug.Log("Honai Seed terakhir terdeteksi. Mengaktifkan collider sebelum dan sesudah hole saat ini.");

            // Cari index hole saat ini
            int currentIndex = colliderHole.colliders.IndexOf(congklakHole.GetComponent<Collider>());
            if (currentIndex == -1)
            {
                Debug.LogError("Hole saat ini tidak ditemukan dalam daftar collider.");
                return;
            }

            // Nonaktifkan semua collider dulu
            foreach (var col in colliderHole.colliders)
            {
                col.enabled = false;
            }

            // Kalau yang diklik adalah Hole 5 (index 4), skip dia
            if (currentIndex == 4)
            {
                // Aktifkan Hole 4 (index 3) dan Hole 6 (index 5)
                colliderHole.colliders[3].enabled = true;
                colliderHole.colliders[5].enabled = true;
            }
            else
            {
                int total = colliderHole.colliders.Count;
                int prevIndex = (currentIndex - 1 + total) % total;
                int nextIndex = (currentIndex + 1) % total;

                // Kalau prev atau next adalah index 4, skip dan tetap aktifkan currentIndex - 2 / + 2 jika perlu
                if (prevIndex == 4) prevIndex = (prevIndex - 1 + total) % total;
                if (nextIndex == 4) nextIndex = (nextIndex + 1) % total;

                colliderHole.colliders[prevIndex].enabled = true;
                colliderHole.colliders[nextIndex].enabled = true;
            }

            // Update UI Collider juga
            colliderHole.UpdateUICollider();
        }
    }

    // Method untuk Biji Spesial (Patung Surabaya)
    // Berfungsi mengubah nilai Target Score sesuai dengan nilai Total Coins ketika diletakkan pada Hole Besar
    // Digunakan pada Method HandleSpecialSeed()
    private void PatungSurabayaSpecialSeed(GameObject seed, CongklakHole congklakHole)
    {
        bool isSpecial = seed.CompareTag("Patung Surabaya Seed");

        if (isSpecial && congklakHole.isScoreSource)
        {
            int currentScore = ScoreManager.Instance.GetCurrentScore();
            int scoreToAdd = 1; // Nilai per biji, sesuaikan kalau beda
            int predictedScore = currentScore + scoreToAdd;

            int currentTarget = StageManager.Instance.GetTargetScoreValue();

            // Jika belum memenuhi target setelah penambahan, maka target bisa diubah
            if (predictedScore < currentTarget)
            {
                int totalCoin = CoinManager.Instance.GetTotalCoins(); // Atau dari mana pun koinnya
                StageManager.Instance.ReplaceTargetScoreWithCoin(totalCoin);
                Debug.Log("Patung Surabaya aktif! Target Score diubah menjadi: " + totalCoin);
            }
            else
            {
                Debug.Log("Patung Surabaya diletakkan saat skor sudah hampir cukup. Efek dibatalkan.");
            }
        }
    }
}
