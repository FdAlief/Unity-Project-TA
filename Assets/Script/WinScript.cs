using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScript : MonoBehaviour
{
    [Header("Script Enable")]
    public MonoBehaviour[] scriptEnable; // Untuk mengaktifkan kembali sistem Raycast

    [Header("UI Elements")]
    public TMP_Text rewardCoinText; // Tampilan UI Text RewardCoin

    private CoinManager coinManager;

    private void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();
    }

    private void Update()
    {
        ShowRewardCoins();
    }

    // Menampilkan jumlah Reward Coin yang diraih pada Panel Win
    // Menggunakan nilai pada script CoinManager (GetLastRewardCoins) 
    private void ShowRewardCoins()
    {
        if (coinManager != null && rewardCoinText != null)
        {
            int rewardCoins = coinManager.GetLastRewardCoins();
            rewardCoinText.text = $"{rewardCoins}";
        }
    }

    // Method untuk mengaktifkan kembali sistem Raycast ketika sudah Win/Lose
    // Digunakan pada Button di Panel Win
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
    }
}
