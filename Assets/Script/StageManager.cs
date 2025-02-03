using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    [Header("Stage Objective")]
    [SerializeField] private int[] targetScore; // Skor yang harus dicapai untuk menyelesaikan stage
    public bool isObjectiveComplete = false; // Apakah objective sudah tercapai
    private int currentTargetIndex; // Indeks target skor yang sedang dicapai
    public TMP_Text targetScoreText; // UI Text untuk menampilkan target score

    [Header("Stage Information")]
    private int currentStage = 1; // Menyimpan informasi stage saat ini, dimulai dari nama stage 1

    [Header("Coin Rewards")]
    [SerializeField] private int[] coinRewards; // Jumlah koin yang diberikan setiap kali target skor tercapai
    private int lastRewardCoins = 0; // Menyimpan reward coin terakhir

    [Header("Win Condition")]
    public GameObject PanelWin; // Panel Win ketika targetScore diraih

    [Header("Win Condition")]
    public GameObject PanelLose; // Panel Lose ketika targetScore tidak dapat diraih dengan TurnCount

    [Header("Script Disable")]
    public MonoBehaviour[] scriptDisable; // Untuk menonaktifkan sistem Raycast ketika Panel Win/Lose muncul

    private CongklakManager congklakManager;

    private InventoryManager inventoryManager;
    private ColliderHoleManager colliderHoleManager;
    private WinScript winScript;
    private LoseScript loseScript;

    private void Start()
    {
        congklakManager = FindObjectOfType<CongklakManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        colliderHoleManager = FindObjectOfType<ColliderHoleManager>();
        winScript = FindObjectOfType<WinScript>();
        loseScript = FindObjectOfType<LoseScript>();

        UpdateTargetScoreUI();
    }

    // Update akan mengecek skor setiap frame
    private void Update()
    {
        CheckObjective();
    }

    // Method untuk mengecek apakah objective sudah tercapai
    private void CheckObjective()
    {
        if (!isObjectiveComplete && ScoreManager.Instance != null)
        {
            if (ScoreManager.Instance.GetCurrentScore() >= targetScore[currentTargetIndex])
            {
                isObjectiveComplete = true;
                OnObjectiveComplete();
            }
        }
    }

    // Aksi yang dilakukan jika objective tercapai
    private void OnObjectiveComplete()
    {
        // Kembalikan menjadi false agar melanjutkan ke objective selanjutnya
        isObjectiveComplete = false;

        // Simpan nilai Reward Coin berdasarkan target score yang tercapai
        lastRewardCoins = coinRewards[currentTargetIndex];

        // Menambahkan total coin manager dari hasil yang didapatkan
        winScript.AddToCoinManager();

        // Aktifkan Panel Win
        PanelWin.SetActive(true);

        // Nonaktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptDisable)
        {
            if (script != null)
            {
                script.enabled = false; // Nonaktifkan script
            }
        }

        // Method untuk mereset biji pada congklak
        congklakManager.ResetSeeds();

        // Method untuk menghapus Inventory
        inventoryManager.ClearInventory();

        // Method untuk mereset collider yang aktif hanya deret player
        colliderHoleManager.ResetCollidersToDefault();

        // Pindah ke targetscore / objective berikutnya jika ada
        if (currentTargetIndex < targetScore.Length - 1)
        {
            currentTargetIndex++;
            currentStage++;
            UpdateTargetScoreUI();
        }

        Debug.Log("Objective Complete! Target Score Reached!");
    }

    // Method untuk menangani kondisi Game Over
    // Digunakan pada script TurnScript ketika sudah mencapai TurnCount
    public void OnGameOver()
    {
        // Memanggil method pada script LoseScript untuk menampilkan UI text Stage Lose
        loseScript.ShowStageOnGameOver();

        // Reset target score ke array pertama (indeks 0) & Stage Informasi ke (index 1)
        currentTargetIndex = 0;
        currentStage = 1;
        UpdateTargetScoreUI();

        // Aktifkan Panel Game Over
        PanelLose.SetActive(true);

        // Nonaktifkan script yang terdaftar (misalnya Raycast atau kontrol)
        foreach (MonoBehaviour script in scriptDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // Method untuk mereset biji pada congklak
        congklakManager.ResetSeeds();

        // Method untuk menghapus Inventory
        inventoryManager.ClearInventory();

        // Method untuk mereset collider yang aktif hanya deret player
        colliderHoleManager.ResetCollidersToDefault();

        Debug.Log("Game Over! Turn sudah maksimal tetapi target skor tidak tercapai.");
    }

    // Method untuk Mengambil Reward Coin Terakhir
    // Digunakan pada Script WinScript (ShowRewardCoins)
    public int GetLastRewardCoins()
    {
        return lastRewardCoins;
    }

    // Method untuk mengambil informasi Stage terakhir sesuai dengan TargetScore
    // Digunakan pada script LoseScript (ShowStageOnGameOver)
    public int GetCurrentStage()
    {
        return currentStage;
    }

    // Method untuk menampilkan TargetScore pada UI
    // Digunakan pada method Start & CheckObjective
    private void UpdateTargetScoreUI()
    {
        if (targetScoreText != null)
        {
            targetScoreText.text = $"{targetScore[currentTargetIndex]}";
        }
    }
}
