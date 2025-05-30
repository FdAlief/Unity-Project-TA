using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CongklakHole : MonoBehaviour
{
    [Header("Isi Hole")]
    [SerializeField]
    public List<GameObject> seedsInHole = new List<GameObject>(); // List biji di setiap lubang

    public int SeedsCount => seedsInHole.Count; // Variabel SeedsCount

    [Header("Score Source")]
    public bool isScoreSource; // Menandai apakah lubang ini adalah sumber untuk jumlah skor

    [Header("Effect Angka UI")]
    [SerializeField] private CounterNumber angkaEffect;

    [Header("Referensi Script")]   
    [SerializeField] private InventoryManager inventoryManager; // Referensi ke manager inventory
    [SerializeField] private SpecialSeedHandler specialSeedHandler; // Referensi ke Special Seed Handler

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
    public void AddSeed(GameObject seed, bool seedFromInventory = false)
    {
        seedsInHole.Add(seed); // Tambahkan ke list seedsInHole
        seed.transform.SetParent(transform); // Set parent ke lubang

        // Memanggil Method untuk pengecekan Biji Spesial
        specialSeedHandler.HandleSpecialSeed(seed, this);

        UpdateSeedCountUI(); // Perbarui UI

        // Shake hanya seed baru jika diminta
        if (seedFromInventory)
        {
             // Jalankan dua efek ke seed baru
            StartCoroutine(MoveSeedToRandomPosition(seed));
        }
        else
        {
            // Default: shake semua
            foreach (GameObject s in seedsInHole)
            {
                if (s != null)
                    StartCoroutine(ShakeSeed(s));
            }
        }
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
    // Digunakan pada method (AddSeed), (TransferSeedsToInventory), (Start) dan Script SpecialSeedHandler
    public void UpdateSeedCountUI()
    {
        if (angkaEffect != null)
        {
            angkaEffect.EffectToAll(SeedsCount); // Jalankan effect dan update angka
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

    // Coroutine untuk membuat efek goyang (shake) pada GameObject seed
    // Digunakan pada Methdo AddSeed()
    private IEnumerator ShakeSeed(GameObject seed, float duration = 0.5f, float magnitude = 1f)
    {
        // Cegah error jika objek seed sudah dihancurkan atau null
        if (seed == null || seed.transform == null) yield break;

        // Simpan posisi awal untuk mengembalikan posisi setelah efek shake selesai
        Vector3 originalPos = seed.transform.localPosition;

        float elapsed = 0f; // Waktu yang telah berlalu selama shake

        // Loop selama waktu shake belum selesai
        while (elapsed < duration)
        {
            // Cek ulang apakah objek masih valid selama loop (bisa saja dihancurkan di tengah efek)
            if (seed == null || seed.transform == null) yield break;

            // Buat posisi acak pada sumbu X dan Y, dikalikan dengan magnitude agar bisa dikontrol seberapa kuat goyangannya
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Ubah posisi seed menjadi posisi awal + offset acak (shake effect)
            seed.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            // Tambahkan waktu yang telah berlalu
            elapsed += Time.deltaTime;

            // Tunggu frame berikutnya
            yield return null;
        }

        // Setelah selesai shake, kembalikan ke posisi semula (hanya jika seed masih ada)
        if (seed != null && seed.transform != null)
            seed.transform.localPosition = originalPos;
    }

    // Coroutine untuk menggerakkan seed secara halus ke posisi acak dalam radius tertentu
    // Digunakan pada Method AddSeed
    private IEnumerator MoveSeedToRandomPosition(GameObject seed, float duration = 0.5f, float radius = 5f)
    {
        // Kalau seed atau transform-nya null (sudah dihancurkan?), hentikan coroutine
        if (seed == null || seed.transform == null) yield break;

        // Posisi awal sebelum bergerak
        Vector3 startPos = seed.transform.localPosition;

        // Buat offset acak dalam bentuk lingkaran 2D (x dan y), lalu dikali radius
        Vector2 randomOffset = Random.insideUnitCircle * radius;

        // Tentukan posisi target berdasarkan offset dari posisi awal
        Vector3 targetPos = startPos + new Vector3(randomOffset.x, randomOffset.y, 0f);

        float elapsed = 0f; // Waktu yang telah berlalu sejak mulai animasi

        // Selama waktu berjalan belum melebihi durasi yang ditentukan
        while (elapsed < duration)
        {
            // Jika seed-nya hilang di tengah jalan, hentikan coroutine
            if (seed == null) yield break;

            // Interpolasi posisi antara start dan target berdasarkan waktu
            seed.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);

            // Tambah waktu berdasarkan deltaTime
            elapsed += Time.deltaTime;

            // Tunggu 1 frame sebelum melanjutkan
            yield return null;
        }

        // Pastikan posisi akhir diset ke target jika belum pas
        if (seed != null)
            seed.transform.localPosition = targetPos;

        // Shake semua seed
        foreach (GameObject s in seedsInHole)
        {
            if (s != null)
            StartCoroutine(ShakeSeed(s));
        }
    }
}