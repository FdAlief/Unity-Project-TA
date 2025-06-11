using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManagerScript : MonoBehaviour
{
    // Singleton instance untuk akses global dan memastikan hanya ada satu AudioManager
    public static AudioManagerScript Instance;

    // Referensi ke AudioSource dan AudioClip yang akan dimainkan
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public float fadeDuration = 1.5f; // Durasi transisi antar musik (dalam detik)

    private Coroutine currentFadeCoroutine; // Menyimpan coroutine aktif agar bisa dihentikan saat perlu ganti musik

    void Awake()
    {
        // Singleton setup — pastikan hanya ada satu AudioManager di scene manapun
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jangan hancurkan saat pindah scene
        }
        else
        {
            Destroy(gameObject); // Hancurkan duplikatnya
            return;
        }

        // Daftar ke event saat scene berubah
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Main Menu biasanya di scene index 0 → setel musik awal
        PlayMenuMusic();
    }

    // Callback saat scene selesai dimuat
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int buildIndex = scene.buildIndex;

        // Kalau scene index 0 atau 1 (Main Menu & Level Menu) → mainkan menuMusic
        if (buildIndex == 0 || buildIndex == 1)
        {
            PlayMusicWithFade(menuMusic);
        }
        else // Index 2 ke atas dianggap scene Gameplay → mainkan gameplayMusic
        {
            PlayMusicWithFade(gameplayMusic);
        }
    }

    // Fungsi helper untuk play musik menu (dipanggil saat awal Start)
    void PlayMenuMusic()
    {
        PlayMusicWithFade(menuMusic);
    }

    // Fungsi utama untuk memainkan musik dengan transisi fade
    void PlayMusicWithFade(AudioClip newClip)
    {
        // Kalau musik yang mau diputar sudah sama dengan yang sedang diputar, skip
        if (musicSource.clip == newClip) return;

        // Hentikan fade sebelumnya kalau ada
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        // Mulai fade musik ke clip baru
        currentFadeCoroutine = StartCoroutine(FadeMusic(newClip));
    }

    // Coroutine untuk mengatur transisi (fade out → ganti lagu → fade in)
    IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        // === FASE 1: Fade Out ===
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        // Hentikan musik, ganti clip, dan mulai mainkan musik baru
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // === FASE 2: Fade In ===
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        // Pastikan volume kembali ke nilai awal
        musicSource.volume = startVolume;
    }
}
