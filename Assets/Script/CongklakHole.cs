using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CongklakHole : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> seedsInHole = new List<GameObject>(); // List biji di setiap lubang

    private InventoryManager inventoryManager; // Referensi ke manager inventory

    public int SeedsCount => seedsInHole.Count;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
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

        Debug.Log("Semua biji dari lubang telah dipindahkan ke inventory.");
    }

    // Method untuk memasukkan data biji ke dalam list data hole atau lubang
    // Digunakan pada script Congklak Manager (PlaceSeedInHole) dan DragHandler (HandleDrag - GetMouseUp/Tounch Ended)
    public void AddSeed(GameObject seed)
    {
        seedsInHole.Add(seed); // Tambahkan ke list seedsInHole
        seed.transform.SetParent(transform); // Set parent ke lubang
    }
}