using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterNumber : MonoBehaviour
{
    [Header("UI Komponen dan Opsi Animasi")]
    [SerializeField] private TextMeshProUGUI valueText; // Komponen teks UI untuk menampilkan angka
    [SerializeField] private float CounterDuration; // Durasi effect perubahan angka
    [SerializeField] private float effectDuration; // Durasi effect Shake, Rotasi dan Scale
    [SerializeField] private bool effectShake; // Apakah teks akan bergoyang
    [SerializeField] private bool effectScale; // Apakah teks akan membesar
    [SerializeField] private bool effectRotate; // Apakah teks akan berotasi

    [Header("Efek Parameter")]
    [SerializeField] private float shakeAmount = 2f; // Jarak shake horizontal
    [SerializeField] private float shakeSpeed = 50f; // Kecepatan shake

    [SerializeField] private float scaleMultiplier = 1.2f; // Ukuran maksimum scale
    [SerializeField] private float scaleCurvePower = 1f; // Mengontrol bentuk kurva scale

    [SerializeField] private float rotationAmount = 5f; // Sudut rotasi maksimal (Z)
    [SerializeField] private float rotationSpeed = 20f; // Kecepatan rotasi

    // Nilai saat ini, coroutine aktif, dan transform asli
    private int currentValue;
    private Coroutine effectCoroutine;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Menyimpan posisi, rotasi, dan scale awal object untuk dipakai ulang saat reset effect
    private void Awake()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    /// Memulai effect perubahan nilai angka ke nilai baru.
    public void EffectToValue(int newValue)
    {
        // Hentikan effect sebelumnya jika masih berjalan
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        // Mulai effect baru
        effectCoroutine = StartCoroutine(EffectValueRoutine(newValue));
    }

    /// Coroutine utama untuk mengubah angka dengan interpolasi + memulai efek visual.
    private IEnumerator EffectValueRoutine(int targetValue)
    {
        int startValue = currentValue;
        float timer = 0f;

        // Mulai efek visual shake/scale/rotate
        StartCoroutine(ShakeRotationScaleRoutine());

        // Interpolasi angka dari startValue ke targetValue selama durasi animasi
        while (timer < CounterDuration)
        {
            timer += Time.deltaTime;
            float t = timer / CounterDuration;

            // Hitung nilai baru berdasarkan waktu
            currentValue = Mathf.FloorToInt(Mathf.Lerp(startValue, targetValue, t));
            valueText.text = currentValue.ToString();

            yield return null;
        }

        // Pastikan nilai akhir sudah benar
        currentValue = targetValue;
        valueText.text = targetValue.ToString();
    }

    /// Coroutine untuk menjalankan efek visual (shake, scale, rotate) saat angka berubah.
    private IEnumerator ShakeRotationScaleRoutine()
    {
        float timer = 0f;
        float duration = effectDuration;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // Effect scale (membesar lalu kembali)
            if (effectScale)
            {
                // effect smooth masuk-keluar
                float scale = Mathf.Lerp(1f, scaleMultiplier, Mathf.Pow(Mathf.Sin(t * Mathf.PI), scaleCurvePower));
                transform.localScale = originalScale * scale;
            }

            // Effect shake horizontal (kanan-kiri)
            if (effectShake)
            {
                float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
                transform.localPosition = originalPosition + new Vector3(offsetX, 0f, 0f);
            }

            // Effect rotasi bolak-balik (efek "goyang")
            if (effectRotate)
            {
                float rotZ = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
                transform.localRotation = Quaternion.Euler(0, 0, rotZ);
            }

            yield return null;
        }

        // Reset ke kondisi awal setelah effect selesai
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    /// Method untuk mengatur nilai awal angka **tanpa effect**, biasanya dipanggil saat Start.
    /// Digunakan pada Script WinScript (OnDisable)
    public void SetInitialValue(int value)
    {
        currentValue = value;
        valueText.text = value.ToString();
    }
}
