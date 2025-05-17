using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipImage : MonoBehaviour
{
    public float flipDuration; // Total waktu animasi rotasi (dari 0 ke 180 derajat)
    public GameObject textNameFront; // Objek teks sisi depan (0 derajat)
    public GameObject textPriceFront; // Objek teks sisi depan (0 derajat)
    public GameObject textBack; // Objek teks sisi belakang (180 derajat)

    private bool isFlipped = false; // Status posisi kartu, true = belakang, false = depan
    private bool isRotating = false; // Lock agar tidak bisa klik saat sedang animasi

    [Header("Referensi Script")]
    [SerializeField] private AudioManagerScript audioManager;

    // Dipanggil saat object disentuh atau diklik
    // Digunakan pada Object "Option Biji Spesial" use Event Trigger
    public void OnTouch()
    {
        if (isRotating) return; // Cegah double tap saat animasi belum selesai
        StartCoroutine(FlipCard()); // Mulai proses rotasi

        // Panggil Audio
        if (audioManager != null)
        {
            audioManager.PlayAudioByIndex(8); // Misalnya index 0 adalah SFX coin
        }
        else
        {
            Debug.LogWarning("AudioManager belum di-assign di inspector!");
        }
    }

    // Coroutine untuk animasi flip kartu 2 arah
    private IEnumerator FlipCard()
    {
        isRotating = true; // Lock input

        float halfDuration = flipDuration / 2f; // Durasi untuk separuh rotasi (0 → 90, 90 → 180)

        // ROTASI AWAL: dari posisi sekarang ke 90 derajat
        Quaternion startRot = transform.rotation;
        Quaternion midRot = Quaternion.Euler(0f, 90f, 0f); // Posisi tengah rotasi
        float elapsed = 0f;

        // Lakukan rotasi ke tengah
        while (elapsed < halfDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, midRot, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = midRot; // Pastikan posisi pas 90°

        // Di tengah animasi: sembunyikan semua teks
        textNameFront.SetActive(false);
        textPriceFront.SetActive(false);
        textBack.SetActive(false);

        // Toggle status flip
        isFlipped = !isFlipped;

        // ROTASI LANJUTAN: 90 ke 180 jika dibalik, atau 90 ke 0 jika kembali
        Quaternion endRot = Quaternion.Euler(0f, isFlipped ? 180f : 0f, 0f);
        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            transform.rotation = Quaternion.Slerp(midRot, endRot, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot; // Pastikan posisi akhir benar

        // AKTIFKAN teks yang sesuai sisi
        if (isFlipped)
        {
            textBack.SetActive(true); // Teks belakang muncul
            textBack.transform.rotation = Quaternion.identity; // Reset rotasi teks
        }
        else
        {
            textNameFront.SetActive(true); // Teks depan muncul
            textPriceFront.SetActive(true);
            textNameFront.transform.rotation = Quaternion.identity;
            textPriceFront.transform.rotation = Quaternion.identity;
        }

        isRotating = false; // Unlock input
    }

    // Method untuk reset posisi ke awal
    // Digunakan pada Button Lanjut di Panel Store
    public void ResetFlip()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Rotasi balik ke depan (0 derajat)
        textNameFront.SetActive(true); // Aktifkan teks depan
        textPriceFront.SetActive(true);
        textNameFront.transform.rotation = Quaternion.identity;
        textPriceFront.transform.rotation = Quaternion.identity;

        textBack.SetActive(false); // Matikan teks belakang

        isFlipped = false; // Status balik ke depan
        isRotating = false; // Unlock input
    }
}
