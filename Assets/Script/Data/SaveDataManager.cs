using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveDataManager
{
    // --- Keys dari CoinManager ---
    public static void ResetCoinData(string keyPrefs)
    {
        PlayerPrefs.DeleteKey(keyPrefs);
        Debug.Log("Coin data dihapus.");
    }

    // --- Keys dari TurnScript ---
    public static void ResetTurnData(string keyPrefs)
    {
        PlayerPrefs.DeleteKey(keyPrefs);
        Debug.Log("Turn data dihapus.");
    }

    // --- Keys dari LevelManager ---
    public static void ResetLevelProgress(string keyPrefs, int totalLevels)
    {
        for (int i = 0; i < totalLevels; i++)
        {
            PlayerPrefs.DeleteKey(keyPrefs + "_unlocked_" + i);
            PlayerPrefs.DeleteKey(keyPrefs + "_completed_" + i);
        }
        Debug.Log("Level progress dihapus.");
    }

    // --- Keys dari StageInput ---
    public static void ResetStageInputData(string keyPrefs, int totalStages)
    {
        for (int i = 0; i < totalStages; i++)
        {
            PlayerPrefs.DeleteKey(keyPrefs + "Unlock_" + i);
            PlayerPrefs.DeleteKey(keyPrefs + "Completed_" + i);
        }
        Debug.Log("StageInput data dihapus.");
    }

    // --- Save All ---
    // Digunakan pada Script MainMenuManager ketika Restart
    public static void ResetAllData(string coinKey, string turnKey, string levelKey, int totalLevels, string stageInputKey, int totalStages)
    {
        ResetCoinData(coinKey);
        ResetTurnData(turnKey);
        ResetLevelProgress(levelKey, totalLevels);
        ResetStageInputData(stageInputKey, totalStages);
        PlayerPrefs.Save();
        Debug.LogWarning("Semua data PlayerPrefs di-reset!");
    }
}
