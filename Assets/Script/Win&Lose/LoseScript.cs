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

    [Header("Informasi Stage")]
    [SerializeField] private string[] infoStage; // Array pesan kustom untuk setiap stage

    [Header("Referensi Script")]
    [SerializeField] private StageManager stageManager;

    // Method untuk menampilkan UI text informasi Stage Lose
    // Digunakan pada script StageManager (OnGameOver)
    public void ShowStageOnGameOver()
    {
        if (stageManager != null && stageText != null)
        {
            int stageIndex = stageManager.GetCurrentTarget(); // Sesuaikan dengan indeks array

            // Pastikan index dalam batas array
            if (stageIndex >= 0 && stageIndex < infoStage.Length)
            {
                stageText.text = infoStage[stageIndex];
            }
            else
            {
                stageText.text = "Game Over! Coba lagi!"; // Default jika tidak ada custom text
            }
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
        TurnScript.Instance.ResetTurnCount();
    }
}
