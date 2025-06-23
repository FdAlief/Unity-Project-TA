using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Key & Stage Info")]
    // KeyPrefs harus sama di Script CoinManager, TurnScript, LevelManager, StageInput
    [SerializeField] private string coinKey;
    [SerializeField] private string turnKey;
    [SerializeField] private string levelKey;
    [SerializeField] private int totalLevels;
    [SerializeField] private string stageInputKey_1;
    [SerializeField] private int totalStages_1;
    [SerializeField] private string stageInputKey_2;
    [SerializeField] private int totalStages_2;
    [SerializeField] private string stageInputKey_3;
    [SerializeField] private int totalStages_3;
    [SerializeField] private string specialseedKey;

    // Digunakan pada button Resume di Main Menu
    public void OnClickResume()
    {
        Debug.Log("Game dilanjutkan!");
    }

    // Digunakan pada button Restart di Main Menu
    public void OnClickRestart()
    {
        // Reset semua Data
        SaveDataManager.ResetAllData(
            coinKey, turnKey, levelKey, totalLevels, 
            stageInputKey_1, stageInputKey_2, stageInputKey_3, 
            totalStages_1, totalStages_2, totalStages_3, specialseedKey
            );

        // Reload semua Data Coin, Turn, Level
        CoinManager.Instance.ReloadCoins();
        TurnScript.Instance.ReloadTurns();
        LevelManager.Instance.ReloadLevelProgress();

        // Buat flag khusus agar InventoryManager tahu dia harus reload SpecialSeed saat scene dibuka
        PlayerPrefs.SetInt("ShouldReloadInventory", 1); // 1 artinya "iya, reload nanti"
        PlayerPrefs.Save();

        Debug.Log("Game di-reset ke awal.");
    }
}
