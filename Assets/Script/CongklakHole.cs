using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CongklakHole : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> seedsInHole = new List<GameObject>(); // List biji di setiap lubang

    private InventoryManager inventoryManager; // Referensi ke manager inventory

    public int SeedsCount => seedsInHole.Count;

    [Header("Info Seed In Hole")]
    public TMP_Text seedCountText; // Referensi ke Text UI

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        UpdateSeedCountUI(); // Perbarui UI saat awal
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
            }
        }

        UpdateSeedCountUI(); // Perbarui UI saat awal
        Debug.Log("Semua biji dari lubang telah dipindahkan ke inventory.");
    }

    // Method untuk memasukkan data biji ke dalam list data hole atau lubang
    // Digunakan pada script Congklak Manager (PlaceSeedInHole) dan DragHandler (HandleDrag - GetMouseUp/Tounch Ended)
    public void AddSeed(GameObject seed)
    {
        seedsInHole.Add(seed); // Tambahkan ke list seedsInHole
        seed.transform.SetParent(transform); // Set parent ke lubang
        UpdateSeedCountUI(); // Perbarui UI saat awal
    }

    // Method ini berfungsi untuk menampilkan jumlah biji di dalam Hole pada UI Text
    // Digunakan pada method (AddSeed), (TransferSeedsToInventory), (Start)
    private void UpdateSeedCountUI()
    {
        if (seedCountText != null)
        {
            seedCountText.text = SeedsCount.ToString(); // Update jumlah biji
        }
    }
}