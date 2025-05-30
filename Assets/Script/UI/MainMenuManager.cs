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
    [SerializeField] private string stageInputKey;
    [SerializeField] private int totalStages;

    // Digunakan pada button Resume di Main Menu
    public void OnClickResume()
    {
        Debug.Log("Game dilanjutkan!");
    }

    // Digunakan pada button Restart di Main Menu
    public void OnClickRestart()
    {
        // Reset semua Data
        SaveDataManager.ResetAllData(coinKey, turnKey, levelKey, totalLevels, stageInputKey, totalStages);

        // Reload semua Data
        CoinManager.Instance.ReloadCoins();
        TurnScript.Instance.ReloadTurns();
        LevelManager.Instance.ReloadLevelProgress();

        Debug.Log("Game di-reset ke awal.");
    }
}
