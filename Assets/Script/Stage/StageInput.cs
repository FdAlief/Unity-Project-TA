using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInput : MonoBehaviour
{
    [Header("Script DIsbale & Enable")]
    public MonoBehaviour[] scriptEnable; // Untuk mengaktifkan kembali sistem Raycast

    [Header("Referensi Script")]
    [SerializeField] private StageManager stageManager;

    private void Start()
    {
        // Aktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = false; // Nonaktifkan script
            }
        }
    }

    // Method untuk memilih Objective yang dijalanka
    // Digunakan pada UI Button Stage Input Menu
    public void ChooseStage(int index)
    {
        // Aktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = true; // Aktifkan script
            }
        }

        if (stageManager != null)
        {
            stageManager.SetTargetScoreIndex(index);
        }
        else
        {
            Debug.LogError("StageManager masih null saat memilih stage!");
        }
    }
}
