using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnScript : MonoBehaviour
{
    private int turnCount = 0; // Jumlah kali inventory terisi
    private bool seedAddedFromHole = false; // Flag untuk cek apakah biji berasal dari hole

    void Update()
    {
        // Jika mengambil biji dari Hole dan hanya hitung jika berasal dari hole
        if (seedAddedFromHole)
        {
            turnCount++;
            Debug.LogWarning("Inventory terisi dari hole, count: " + turnCount);
            seedAddedFromHole = false; // Reset flag setelah dihitung
        }
    }

    // Method ini dipanggil dari Hole saat transfer biji ke inventory
    // Untuk patokan agar turnCount bertambah jika mengambil biji dari Hole ke Inventory
    // Digunakan pada script CongklakHole (TransferSeedsToInventory)
    public void OnSeedAddedFromHole()
    {
        seedAddedFromHole = true;
    }

    public int GetFillCount()
    {
        return turnCount; // Mengembalikan jumlah kali inventory terisi
    }
}
