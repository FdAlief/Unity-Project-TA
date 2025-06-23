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
    public static void ResetStageInputData_1(string keyPrefs, int totalStages)
    {
        for (int i = 0; i < totalStages; i++)
        {
            PlayerPrefs.DeleteKey(keyPrefs + "Unlock_" + i);
            PlayerPrefs.DeleteKey(keyPrefs + "Completed_" + i);
        }
        Debug.Log("StageInput data dihapus.");
    }

    public static void ResetStageInputData_2(string keyPrefs, int totalStages)
    {
        for (int i = 0; i < totalStages; i++)
        {
            PlayerPrefs.DeleteKey(keyPrefs + "Unlock_" + i);
            PlayerPrefs.DeleteKey(keyPrefs + "Completed_" + i);
        }
        Debug.Log("StageInput data dihapus.");
    }

    public static void ResetStageInputData_3(string keyPrefs, int totalStages)
    {
        for (int i = 0; i < totalStages; i++)
        {
            PlayerPrefs.DeleteKey(keyPrefs + "Unlock_" + i);
            PlayerPrefs.DeleteKey(keyPrefs + "Completed_" + i);
        }
        Debug.Log("StageInput data dihapus.");
    }

    // --- Keys dari InventoryManager ---
    public static void ResetSpecialSeedData(string keyPrefs)
    {
        PlayerPrefs.DeleteKey(keyPrefs);
        Debug.Log("Special Seed data dihapus.");
    }

    // --- Save All ---
    // Digunakan pada Script MainMenuManager ketika Restart
    public static void ResetAllData(
        string coinKey, string turnKey, string levelKey, int totalLevels, 
        string stageInputKey_1, string stageInputKey_2, string stageInputKey_3,
        int totalStages_1, int totalStages_2, int totalStages_3,
        string specialseedKey)
    {
        ResetCoinData(coinKey);
        ResetTurnData(turnKey);
        ResetLevelProgress(levelKey, totalLevels);
        ResetStageInputData_1(stageInputKey_1, totalStages_1);
        ResetStageInputData_2(stageInputKey_2, totalStages_2);
        ResetStageInputData_3(stageInputKey_3, totalStages_3);
        ResetSpecialSeedData(specialseedKey);
        PlayerPrefs.Save();
        Debug.LogWarning("Semua data PlayerPrefs di-reset!");
    }
}
