using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoseScript : MonoBehaviour
{
    [Header("Script Enable")]
    public MonoBehaviour[] scriptEnable; // Untuk me-aktifkan sistem Raycast

    [Header("UI Elements")]
    public TMP_Text stageText; // Tambahkan UI untuk menampilkan skor

    private TurnScript turnScript;
    private StageManager stageManager;

    private void Start()
    {
        turnScript = FindObjectOfType<TurnScript>();
        stageManager = FindObjectOfType<StageManager>();
    }


    // Method untuk menampilkan UI text informasi Stage Lose
    // Digunakan pada script StageManager (OnGameOver)
    public void ShowStageOnGameOver()
    {
        if (stageManager != null && stageText != null)
        {
            stageText.text = $"Kamu Kalah di Stage {stageManager.GetCurrentStage()}";
        }
    }

    // Method untuk mengaktifkan kembali sistem Raycast ketika sudah Win/Lose
    // Digunakan pada Button di Panel Lose
    public void ActiveRaycast()
    {
        // Aktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = true; // Aktifkan script
            }
        }

        // Reset turnCount setiap kali objective tercapai
        turnScript.ResetTurnCount();
    }
}
