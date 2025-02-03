using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Text Score")]
    public TMP_Text scoreText; // Referensi ke Text UI untuk skor

    private int currentScore = 0; // Score yang sedang diraih
    private int lastScore = 0; // Score terkahir yang diraih

    private void Start()
    {
        UpdateScoreUI(); // Tampilkan skor awal
    }

    // Method untuk mengubah jumlah score yang didapatkan
    // Digunakan pada script CongklakHole (UpdateScore)
    public void SetScore(int newScore)
    {
        lastScore = currentScore; // Simpan skor terakhir sebelum diubah
        currentScore = newScore;  // Update skor baru
        UpdateScoreUI();
    }

    // Method untuk mengubah UI text tampilan Score
    // Digunakan pada method SetScore & Start
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString(); ; // Update teks UI
        }
    }

    // Method untuk mendapatkan skor saat ini
    // Digunakan pada script StageManager ketika score meraih target score akan selesai stage
    public int GetCurrentScore()
    {
        return currentScore;
    }

    // Method untuk mendapatkan skor terakhir yang diraih
    // Digunakan pada Script WinScript (ShowScore) ditampilkan ketika Win
    public int GetLastScore()
    {
        return lastScore;
    }
}
