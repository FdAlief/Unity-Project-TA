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
    [SerializeField] private bool effectOpacity; // Apakah teks akan fade in/out

    [Header("Efek Parameter")]
    [SerializeField] private float shakeAmount; // Jarak shake horizontal
    [SerializeField] private float shakeSpeed; // Kecepatan shake

    [SerializeField] private float scaleMultiplier; // Ukuran maksimum scale
    [SerializeField] private float scaleCurvePower; // Mengontrol bentuk kurva scale

    [SerializeField] private float rotationAmount; // Sudut rotasi maksimal (Z)
    [SerializeField] private float rotationSpeed; // Kecepatan rotasi

    [SerializeField] private float displayDuration; // Waktu tampilan

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
    public void EffectToCount(int newValue)
    {
        // Hentikan effect sebelumnya jika masih berjalan
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        // Mulai effect baru
        effectCoroutine = StartCoroutine(CountRoutine(newValue));
    }

    /// Memulai effect Shake, Rotate, Scale.
    public void EffectToShake()
    {
        // Hentikan effect sebelumnya jika masih berjalan
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        // Mulai effect baru
        effectCoroutine = StartCoroutine(ShakeRotationScaleRoutine());
    }

    /// Memulai effect Shake, Rotate, Scale.
    public void EffectToAll(int newValue)
    {
        // Hentikan effect sebelumnya jika masih berjalan
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        // Mulai effect baru
        effectCoroutine = StartCoroutine(ShakeRotationScaleRoutine());
        effectCoroutine = StartCoroutine(CountRoutine(newValue));
    }

    /// Coroutine utama untuk mengubah angka dengan interpolasi + memulai efek visual.
    private IEnumerator CountRoutine(int targetValue)
    {
        int startValue = currentValue;
        float timer = 0f;

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
        if (effectOpacity)
        {
            StartCoroutine(FadeInOutRoutine());
        }

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

    // Coroutine untuk effect Fade in / out
    // Digunakan pada Coroutine ShakeRotationScaleRoutine()
    private IEnumerator FadeInOutRoutine()
    {
        float timer = 0f;

        // Fade In
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float t = timer / effectDuration;
            valueText.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        valueText.alpha = 1f;

        // Tahan teks selama displayDuration
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        timer = 0f;
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float t = timer / effectDuration;
            valueText.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        valueText.alpha = 0f;
    }

    /// Method untuk mengatur nilai awal angka **tanpa effect**, biasanya dipanggil saat Start.
    /// Digunakan pada Script WinScript (OnDisable)
    public void SetInitialValue(int value)
    {
        currentValue = value;
        valueText.text = value.ToString();
    }
}
