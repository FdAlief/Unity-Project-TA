using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnScript : MonoBehaviour
{
    [SerializeField]
    private int turnCount = 0; // Jumlah kali inventory terisi
    [SerializeField]
    private int maxTurns = 3; // Batas maksimal turnCount
    private bool seedAddedFromHole; // Tanda untuk cek apakah biji berasal dari hole

    private StageManager stageManager;

    [Header("UI Turn Counter")]
    public TMP_Text turnCountText; // UI Text untuk menampilkan turn saat ini

    private void Start()
    {
        stageManager = FindObjectOfType<StageManager>();

        UpdateTurnUI(); // Perbarui UI pertama kali
    }

    void Update()
    {
        // Jika mengambil biji dari Hole dan hanya hitung jika berasal dari hole
        if (seedAddedFromHole)
        {
            turnCount++;
            Debug.LogWarning("Inventory terisi dari hole, count: " + turnCount);
            seedAddedFromHole = false; // Reset flag setelah dihitung
            UpdateTurnUI(); // Perbarui UI

            // Jika turnCount mencapai batas maksimal, lakukan aksi
            if (turnCount > maxTurns)
            {
                if (!stageManager.isObjectiveComplete) // Jika belum menang, maka game over
                {
                    stageManager.OnGameOver();
                }
                else
                {
                    Debug.LogWarning("Turn Sudah Maksimal, tapi menang!");
                }
            }
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

    // Method baru untuk mereset turnCount ke 0 ketika sudah Win/Lose
    // Digunakan pada method StageManager (OnObjectiveComplete & OnGameOver)
    public void ResetTurnCount()
    {
        turnCount = 0;
        UpdateTurnUI(); // Perbarui UI
        Debug.Log("Turn count telah di-reset ke 0.");
    }

    // Method untuk menampilkan TurnCount pada UI dengan teks
    private void UpdateTurnUI()
    {
        if (turnCountText != null)
        {
            turnCountText.text = $"Turn : {turnCount} / {maxTurns}";
        }
    }
}
