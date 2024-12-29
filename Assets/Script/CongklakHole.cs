using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CongklakHole : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> seedsInHole = new List<GameObject>(); // List biji di lubang
    public InventoryManager inventoryManager; // Referensi ke manager inventory

    public void HandleClick()
    {
        TransferSeedsToInventory();
    }

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

    public void AddSeed(GameObject seed)
    {
        seedsInHole.Add(seed);  // Tambahkan biji ke dalam list seedsInHole
    }
}
