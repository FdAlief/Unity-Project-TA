using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CongklakHole : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> seedsInHole = new List<GameObject>(); // List biji di setiap lubang

    public InventoryManager inventoryManager; // Referensi ke manager inventory

    // Method untuk melakukan Transfer atau pemindahan biji ke Inventory
    // Method ini digunakan juga pada script Raycast Manager (TakeSeedToInventory)
    public void HandleClick()
    {
        TransferSeedsToInventory();
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
    // Digunakan pada script Congklak Manager (PlaceSeedInHole)
    public void AddSeed(GameObject seed)
    {
        seedsInHole.Add(seed);  // Tambahkan biji ke dalam list seedsInHole
    }
}
