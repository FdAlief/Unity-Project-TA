using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CongklakHole : MonoBehaviour
{
    [Header("Isi Hole")]
    [SerializeField]
    private List<GameObject> seedsInHole = new List<GameObject>(); // List biji di setiap lubang

    public int SeedsCount => seedsInHole.Count; // Variabel SeedsCount

    [Header("Info Seed In Hole")]
    public TMP_Text seedCountText; // Referensi ke Text UI

    [Header("Score Source")]
    public bool isScoreSource; // Menandai apakah lubang ini adalah sumber untuk jumlah skor

    [Header("Data Biji")]
    [SerializeField] private SeedConfig seedConfig; // Data biji default dan spesial

    [Header("Referensi Script")]   
    [SerializeField] private InventoryManager inventoryManager; // Referensi ke manager inventory
    [SerializeField] private CongklakManager congklakManager ; // Referensi ke congklak manager

    private void Start()
    {
        UpdateSeedCountUI(); // Perbarui UI saat awal

        // Jika lubang ini adalah sumber skor, update skor awal
        if (isScoreSource)
        {
            UpdateScore();
        }
    }

    // Method untuk melakukan Transfer atau pemindahan biji ke Inventory
    // Method ini digunakan juga pada script Raycast Manager (TakeSeedToInventory) dan Drag Handler (HandleDrag - MouseUp/Touch Ended)
    public void HandleClick()
    {
        if (inventoryManager.IsInventoryEmpty())
        {
            TransferSeedsToInventory();
        }
        else
        {
            Debug.LogWarning("Tidak dapat memindahkan biji: Inventory tidak kosong.");
        }
    }

    // Method untuk memindahkan biji dari lubang ke Inventory
    // Digunakan pada method HandleClick
    private void TransferSeedsToInventory()
    {
        for (int i = seedsInHole.Count - 1; i >= 0; i--) // Loop mundur untuk aman saat menghapus item
        {
            GameObject seed = seedsInHole[i];
            if (inventoryManager.AddSeedToInventory(seed)) // Hanya jika berhasil dimasukkan ke inventory
            {
                seedsInHole.RemoveAt(i); // Hapus dari list seedsInHole

                // Penghitungan turnCount
                TurnScript.Instance.OnSeedAddedFromHole(); // Tandai bahwa seed berasal dari hole
            }
        }

        UpdateSeedCountUI(); // Perbarui UI saat awal
        Debug.Log("Semua biji dari lubang telah dipindahkan ke inventory.");
    }

    // Method untuk memindahkan seluruh biji ke lubang tertentu (Hole Besar)
    // Disini method ini digunakan pada Script DragHandler (MouseUp)
    public void TransferSeedsToSpecificHole(CongklakHole targetHole)
    {
        if (targetHole == null)
        {
            Debug.LogError("Target hole tidak ditemukan!");
            return;
        }

        // Pindahkan dan hancurkan setiap biji dari hole asal
        for (int i = seedsInHole.Count - 1; i >= 0; i--)
        {
            GameObject seed = seedsInHole[i];

            // Buat duplikasi biji untuk target hole
            GameObject seedUpdate = Instantiate(seed, targetHole.transform.position, Quaternion.identity, targetHole.transform);

            // Tambahkan duplikasi biji ke target hole
            targetHole.AddSeed(seedUpdate);

            // Hapus biji asli dari scene
            Destroy(seed);

            // Hapus biji dari list seedsInHole (hole asal)
            seedsInHole.RemoveAt(i);
        }

        // Perbarui UI pada hole ini
        UpdateSeedCountUI();

        // Perbarui UI pada target hole (jika diperlukan)
        targetHole.UpdateSeedCountUI();

        Debug.Log($"Semua biji dari {gameObject.name} telah dipindahkan ke {targetHole.gameObject.name}.");
    }

    // Method untuk memasukkan data biji ke dalam list data hole atau lubang
    // Digunakan pada script Congklak Manager (PlaceSeedInHole) dan DragHandler (HandleDrag - GetMouseUp/Tounch Ended)
    public void AddSeed(GameObject seed)
    {
        seedsInHole.Add(seed); // Tambahkan ke list seedsInHole
        seed.transform.SetParent(transform); // Set parent ke lubang
        
        // Panggil method Special Seed (Monas) untuk melipat gandakan biji pada Hole biasa
        MonasSpecialSeed(seed);

        // Panggil method Special Seed (Komodo) untuk melipat gandakan biji pada Hole besar
        KomodoSpecialSeed(seed);

        // Panggil method Special Seed (Bali) untuk menambahkan biji random (1-10) pada Hole besar
        BaliSpecialSeed(seed);

        UpdateSeedCountUI(); // Perbarui UI
    }

    // Method untuk menghapus semua biji dari lubang
    // Digunakan pada script CongklakManager
    public void ClearSeeds()
    {
        for (int i = seedsInHole.Count - 1; i >= 0; i--)
        {
            GameObject seed = seedsInHole[i];
            Destroy(seed); // Hapus biji dari scene
        }

        seedsInHole.Clear(); // Bersihkan list seedsInHole
        UpdateSeedCountUI(); // Perbarui UI
    }

    // Method ini berfungsi untuk menampilkan jumlah biji di dalam Hole pada UI Text
    // Digunakan pada method (AddSeed), (TransferSeedsToInventory), (Start)
    private void UpdateSeedCountUI()
    {
        if (seedCountText != null)
        {
            seedCountText.text = SeedsCount.ToString(); // Update jumlah biji
        }

        // Jika lubang ini adalah sumber skor, perbarui skor global
        if (isScoreSource)
        {
            UpdateScore();
        }
    }

    // Method untuk mengatur score yang di dapatkan berdasarkan biji pada Hole (Hole Besar)
    // Digunakan pada method UpdateSeedCountUI() & Start
    private void UpdateScore()
    {
        // Memperbarui skor di ScoreManager berdasarkan SeedsCount
        ScoreManager.Instance.SetScore(SeedsCount);
    }

    // Method untuk Biji Spesial (Monas)
    // Berfungsi melipat gandakan biji hanya pada Hole biasa, tidak pada Hole Besar dan bukan Biji terahir
    // Digunakan pada Method AddSeed()
    private void MonasSpecialSeed(GameObject seed)
    {
        // Cek apakah seed adalah seed special dan bukan pada hole sumber skor dan bukan biji terakhir
        bool isSpecial = seed.CompareTag("Monumen Nasional Seed");
        bool isLastSeed = inventoryManager.IsLastSeed(seed);

        if (isSpecial && !isScoreSource && !isLastSeed)
        {
            Debug.Log("Monas Seed Special terdeteksi dan akan menggandakan biji di Hole biasa");

            int jumlahAsli = seedsInHole.Count - 1; // Kurangi 1 karena seed special baru saja ditambahkan

            for (int i = 0; i < jumlahAsli; i++)
            {
                GameObject originalSeed = seedsInHole[i];
                GameObject duplicatedSeed = Instantiate(originalSeed, transform.position, Quaternion.identity, transform);
                seedsInHole.Add(duplicatedSeed);
            }

            UpdateSeedCountUI(); // Update UI setelah menggandakan biji
        }
    }

    // Method untuk Biji Spesial (Komodo)
    // Berfungsi melipat gandakan biji hanya pada Hole Besar
    // Digunakan pada Method AddSeed()
    private void KomodoSpecialSeed(GameObject seed)
    {
        // Cek apakah seed adalah seed special dan berada pada hole sumber skor (Hole Besar)
        bool isSpecial = seed.CompareTag("Komodo Seed");

        if (isSpecial && isScoreSource)
        {
            Debug.Log("Komodo Seed Special terdeteksi dan akan menggandakan biji di Hole Besar");

            int jumlahAsli = seedsInHole.Count - 1; // Kurangi 1 karena seed special baru saja ditambahkan

            for (int i = 0; i < jumlahAsli; i++)
            {
                GameObject originalSeed = seedsInHole[i];
                GameObject duplicatedSeed = Instantiate(originalSeed, transform.position, Quaternion.identity, transform);
                seedsInHole.Add(duplicatedSeed);
            }

            UpdateSeedCountUI(); // Update UI setelah menggandakan biji
        }
    }

    // Method untuk Biji Spesial (Bali)
    // Berfungsi menambahkan biji random (1-10) hanya pada Hole Besar
    // Digunakan pada Method AddSeed()
    private void BaliSpecialSeed(GameObject seed)
    {
        // Cek apakah seed adalah seed special dan berada pada hole sumber skor (Hole Besar)
        bool isSpecial = seed.CompareTag("Bali Seed");

        if (isSpecial && isScoreSource)
        {
            Debug.Log("Bali Seed terdeteksi. Menambahkan biji random ke Hole.");

            int jumlahRandom = Random.Range(1, 11); // 1 hingga 10 biji baru (karena max exclusive)

            for (int i = 0; i < jumlahRandom; i++)
            {
                // Melakukan penambahan dan peletakkan biji default pada Hole Besar
                congklakManager.PlaceSeedInHole(transform, seedConfig.defaultSeedPrefab);
            }

            UpdateSeedCountUI(); // Update UI setelah menambahkan biji
        }
    }
}