using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Instance agar ketika digunakan pada script lain tidak perlu mencari referensi
    // hanya satu baris kode "ScoreManager.Instance.SetScore(SeedsCount);"
    public static ScoreManager Instance;

    [Header("UI Text Score")]
    [SerializeField] private TMP_Text scoreText; // Referensi ke Text UI untuk skor

    private int currentScore = 0; // Score yang sedang diraih
    private int lastScore = 0; // Score terkahir yang diraih

    // Untuk memastikan hanya ada 1 instance, jika lebih di destroy
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
