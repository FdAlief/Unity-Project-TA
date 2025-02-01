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

    [Header("Win Condition")]
    public GameObject PanelWin; // Panel Win ketika targetScore diraih

    [Header("Win Condition")]
    public GameObject PanelLose; // Panel Lose ketika targetScore tidak dapat diraih dengan TurnCount

    [Header("Script Disable")]
    public MonoBehaviour[] scriptDisable; // Untuk menonaktifkan sistem Raycast ketika Panel Win/Lose muncul

    private CongklakManager congklakManager;
    private InventoryManager inventoryManager;
    private ColliderHoleManager colliderHoleManager;
    private TurnScript turnScript;

    private void Start()
    {
        congklakManager = FindObjectOfType<CongklakManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        colliderHoleManager = FindObjectOfType<ColliderHoleManager>();
        turnScript = FindObjectOfType<TurnScript>();

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
                currentTargetIndex++;
                UpdateTargetScoreUI();
            }
        }
    }

    // Aksi yang dilakukan jika objective tercapai
    private void OnObjectiveComplete()
    {
        // Kembalikan menjadi false agar melanjutkan ke objective selanjutnya
        isObjectiveComplete = false;

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

        // Reset turnCount setiap kali objective tercapai
        turnScript.ResetTurnCount();

        Debug.Log("Objective Complete! Target Score Reached!");
    }

    // Method untuk menangani kondisi Game Over
    // Digunakan pada script TurnScript ketika sudah mencapai TurnCount
    public void OnGameOver()
    {
        // Reset target score ke array pertama (indeks 0)
        currentTargetIndex = 0;
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

        // Reset turnCount setiap kali objective tercapai
        turnScript.ResetTurnCount();

        Debug.Log("Game Over! Turn sudah maksimal tetapi target skor tidak tercapai.");
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
