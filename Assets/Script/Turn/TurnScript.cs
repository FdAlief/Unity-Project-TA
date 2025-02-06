using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnScript : MonoBehaviour
{
    public static TurnScript Instance; // Singleton agar mudah diakses
    public event Action<int> OnTurnChanged; // Event untuk update UI

    [Header("Turn Play")]
    [SerializeField] private int turnCount = 0; // Jumlah kali inventory terisi
    [SerializeField] private int maxTurns = 3; // Batas maksimal turnCount
    [SerializeField] private int coinRewardPerTurn = 2; // Nilai coin per sisa turn

    private bool seedAddedFromHole; // Tanda untuk cek apakah biji berasal dari hole

    private StageManager stageManager; // Referensi Script

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        stageManager = FindObjectOfType<StageManager>();

        // Panggil event pertama kali untuk update UI awal
        OnTurnChanged?.Invoke(turnCount);
    }

    void Update()
    {
        // Jika mengambil biji dari Hole dan hanya hitung jika berasal dari hole
        if (seedAddedFromHole)
        {
            turnCount++;
            Debug.LogWarning("Inventory terisi dari hole, count: " + turnCount);
            seedAddedFromHole = false; // Reset flag setelah dihitung

            // Panggil event untuk memperbarui UI
            OnTurnChanged?.Invoke(turnCount);

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

    public int GetTurnCount()
    {
        return turnCount;
    }

    public int GetMaxTurns()
    {
        return maxTurns;
    }

    // Method baru untuk mereset turnCount ke 0 ketika sudah Win/Lose
    // Digunakan pada method StageManager (OnObjectiveComplete & OnGameOver)
    public void ResetTurnCount()
    {
        turnCount = 0;
        OnTurnChanged?.Invoke(turnCount);
        Debug.Log("Turn count telah di-reset ke 0.");
    }

    // Method untuk mendapatkan sisa turn
    // Digunakan pada script WinScript
    public int GetRemainingTurns()
    {
        int remainingTurns = maxTurns - turnCount;
        return remainingTurns < 0 ? 0 : remainingTurns;
    }

    // Method untuk mendapatkan total coin dari sisa turn
    // Digunakan pada script WinScript
    public int GetRemainingTurnCoins()
    {
        return GetRemainingTurns() * coinRewardPerTurn;
    }
}
