using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Objective")]
    [SerializeField] private int targetScore; // Skor yang harus dicapai untuk menyelesaikan stage
    private bool isObjectiveComplete = false; // Apakah objective sudah tercapai

    [Header("Win Condition")]
    public GameObject PanelWin; // Panel Win ketika targetScore diraih
    public MonoBehaviour[] scriptDisable; // Untuk menonaktifkan sistem Raycast ketika Panel Win muncul

    private CongklakManager congklakManager;
    private InventoryManager inventoryManager;
    private ColliderHoleManager colliderHoleManager;

    private void Start()
    {
        congklakManager = FindObjectOfType<CongklakManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        colliderHoleManager = FindObjectOfType<ColliderHoleManager>();
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
            if (ScoreManager.Instance.GetCurrentScore() >= targetScore)
            {
                isObjectiveComplete = true;
                OnObjectiveComplete();
            }
        }
    }

    // Aksi yang dilakukan jika objective tercapai
    private void OnObjectiveComplete()
    {
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

        Debug.Log("Objective Complete! Target Score Reached!");
    }
}
