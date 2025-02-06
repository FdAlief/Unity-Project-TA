using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton agar bisa diakses dari mana saja

    private int currentScore = 0; // Skor saat ini
    private int lastScore = 0; // Skor terakhir yang diraih

    public event Action<int> OnScoreChanged; // Event untuk update UI

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

    // Method untuk mengubah jumlah score yang didapatkan
    // Digunakan pada script CongklakHole (UpdateScore)
    public void SetScore(int newScore)
    {
        lastScore = currentScore; // Simpan skor terakhir sebelum diubah
        currentScore = newScore;  // Update skor baru

        // Panggil event untuk memperbarui UI
        OnScoreChanged?.Invoke(currentScore);
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
