using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScript : MonoBehaviour
{
    [Header("Script Enable & Disable")]
    public MonoBehaviour[] scriptEnable; // Untuk mengaktifkan kembali sistem Raycast

    [Header("UI Elements")]
    public TMP_Text scoreText; // Tambahkan UI untuk menampilkan skor
    public Button nextButton;

    [Header("Next Scene Level")]
    public string sceneNextLevel;

    [Header("Effect Angka UI")]
    [SerializeField] private CounterNumber rewardCoinEffect;
    [SerializeField] private CounterNumber sisaTurnEffect;
    [SerializeField] private CounterNumber rewardTurnEffect;
    [SerializeField] private CounterNumber totalEffect;

    [Header("Referensi Script")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private CongklakManager congklakManager;
    [SerializeField] private SFXAudio sfxAudio;

    void Update()
    {
        ShowScore();
    }

    // Method untuk menjalankan Coroutine ketika Panel Win Enable
    void OnEnable()
    {
        StartCoroutine(DelayShowUI(0.5f, 1.5f));
    }

    // Method untuk mereset Hasil menjadi 0 ketika Panel Win Disable
    void OnDisable()
    {
        rewardCoinEffect.SetInitialValue(0);
        sisaTurnEffect.SetInitialValue(0);
        rewardTurnEffect.SetInitialValue(0);
        totalEffect.SetInitialValue(0);
        nextButton.interactable = false;
    }

    // Coroutine untuk menampilkan Hasil yang diraih beseta Effect
    // Digunakan pada Method OnEnable
    private IEnumerator DelayShowUI(float delay1, float delay2)
    {
        yield return new WaitForSeconds(delay1);
        ShowRewardCoins();
        yield return new WaitForSeconds(delay2);
        ShowRemainingTurn();
        yield return new WaitForSeconds(delay2);
        ShowRewardTurn();
        yield return new WaitForSeconds(delay2);
        ShowTotalCoins();
        yield return new WaitForSeconds(delay2);
        nextButton.interactable = true;
    }

    // Menampilkan skor yang diraih saat menang
    // Mengguanakn method GetLastScore() dari script ScoreManager
    private void ShowScore()
    {
        if (ScoreManager.Instance != null && scoreText != null)
        {
            int lastScore = ScoreManager.Instance.GetLastScore(); // Ambil skor terakhir
            scoreText.text = $"{lastScore}";
        }
    }

    // Menampilkan jumlah Reward Coin yang diraih pada Panel Win
    // Menggunakan nilai pada script StageManager (GetLastRewardCoins) 
    private void ShowRewardCoins()
    {
        if (stageManager != null && rewardCoinEffect != null)
        {
            int rewardCoins = stageManager.GetLastRewardCoins(); // Ambil nilai reward dari StageManager
            rewardCoinEffect.EffectToAll(rewardCoins); // UI tampilan beserta Effect
        }
    }

    // Method untuk menampilkan sisa turn di UI
    // Menggunakan method pada Script TurnScript
    private void ShowRemainingTurn()
    {
        if (TurnScript.Instance != null && sisaTurnEffect != null)
        {
            int sisaTurn = TurnScript.Instance.GetRemainingTurns();

            // UI tampilan beserta Effect
            sisaTurnEffect.EffectToAll(sisaTurn); 

        }
    }

    // Method untuk menampilkan reward coin dari sisa turn (reward turn coin) di UI
    // Menggunakan method pada Script TurnScript
    private void ShowRewardTurn()
    {
        if (TurnScript.Instance != null && rewardTurnEffect != null)
        {
            int rewardCoinsTurn = TurnScript.Instance.GetRemainingTurnCoins();

            // UI tampilan beserta Effect
            rewardTurnEffect.EffectToAll(rewardCoinsTurn);

        }
    }

    // Method untuk menampilkan Total Koin (Reward + Coin Sisa Turn)
    // Menggunakan method pada Script TurnScript & StageManager
    private void ShowTotalCoins()
    {
        if (totalEffect != null && TurnScript.Instance != null && stageManager != null)
        {
            int rewardCoins = stageManager.GetLastRewardCoins(); // Reward dari StageManager
            int rewardCoinsTurn = TurnScript.Instance.GetRemainingTurnCoins(); // Reward dari sisa turn

            int totalCoins = rewardCoins + rewardCoinsTurn; // Total keseluruhan

            totalEffect.EffectToAll(totalCoins); // UI tampilan beserta Effect
        }
    }

    // Method untuk menambahkan atau memasukkan total coin ke data CoinManager
    // Ketika menyelesaikan objective
    // Digunakan pada script StageManager (OnObjectiveComplete)
    public void AddToCoinManager()
    {
        if (totalEffect != null && TurnScript.Instance != null && stageManager != null)
        {
            int rewardCoins = stageManager.GetLastRewardCoins(); // Reward dari StageManager
            int rewardCoinsTurn = TurnScript.Instance.GetRemainingTurnCoins(); // Reward dari sisa turn

            int totalCoins = rewardCoins + rewardCoinsTurn; // Total keseluruhan

            // Memasukkan / menambahkan ke dalam data Total Coin Manager
            CoinManager.Instance.AddCoins(totalCoins);

            // Panggil Audio
            if (sfxAudio != null)
            {
                sfxAudio.PlayAudioByIndex(3); // Misalnya index 0 adalah SFX coin
            }
            else
            {
                Debug.LogWarning("AudioManager belum di-assign di inspector!");
            }
        }
    }

    // Method untuk mengaktifkan kembali sistem Raycast ketika sudah Win/Lose
    // Digunakan pada Button di Panel Store setelah dari Panel Win
    public void ContinueGame()
    {
        // Aktifkan script yang terdaftar
        foreach (MonoBehaviour script in scriptEnable)
        {
            if (script != null)
            {
                script.enabled = true; // Aktifkan script
            }
        }

        // Reset turnCount
        TurnScript.Instance.ResetTurnCount();

        // Reset Seed pada Congklak
        congklakManager.ResetSeeds();

        // Pindah scene ke Level berikutnya ketika menyelesaikan Level sebelumnya
        if (stageManager != null && stageManager.isFinalTargetReached)
        {
            sfxAudio.PlayAudioByIndex(0); // Play SFX
            SceneManager.LoadScene(sceneNextLevel); // Ganti dengan nama scene tujuan
        }
        else
        {
            sfxAudio.PlayAudioByIndex(4); // Play SFX
            Debug.Log("Belum mencapai target terakhir! Tidak bisa pindah scene.");
        }
    }
}
