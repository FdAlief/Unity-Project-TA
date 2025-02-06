using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    [Header("PlayerPrefs Key to Save Data Coin")]
    [SerializeField] private string keyPrefs; // Key prefs yang bisa diubah di Inspector

    public static CoinManager Instance; // Singleton agar mudah diakses
    private int totalCoins; // Menyimpan jumlah total koin

    public event Action<int> OnCoinChanged; // Event untuk update UI

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
        LoadCoins(); // Load koin saat game mulai
    }

    // Method untuk menambahkan nilai Coin yang didapatkan
    // Digunakan pada script WinScript (AddToCoinManager)
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveCoins(); // Simpan koin setiap kali bertambah
        OnCoinChanged?.Invoke(totalCoins); // Panggil event saat koin bertambah

        Debug.Log($"Koin Ditambahkan: {amount}, Total Koin Sekarang: {totalCoins}");
    }

    // Method untuk mendapatkan total coin
    // Digunakan pada script GameplayUIScript (UITextCoin)
    public int GetTotalCoins()
    {
        return totalCoins;
    }

    // Simpan total coin ke PlayerPrefs
    private void SaveCoins()
    {
        PlayerPrefs.SetInt($"{keyPrefs}", totalCoins);
        PlayerPrefs.Save();
        Debug.Log("Total Koin Disimpan: " + totalCoins);
    }

    // Load total coin dari PlayerPrefs
    private void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt($"{keyPrefs}", 0); // 0 jika belum ada data
        Debug.Log("Total Koin Dimuat: " + totalCoins);
        OnCoinChanged?.Invoke(totalCoins); // Update UI setelah load
    }
}
