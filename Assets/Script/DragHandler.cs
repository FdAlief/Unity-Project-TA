using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour
{
    private GameObject selectedSeed; // Objek yang sedang di-drag
    private Vector3 offset; // Offset posisi antara mouse dan objek
    private Camera mainCamera; // Kamera utama
    private RaycastManager raycastManager; // Referensi ke RaycastManager
    private InventoryManager inventoryManager; // Referensi ke InventoryManager
    private Vector3 initialPosition; // Posisi awal biji di slot inventory
    private Vector3 initialScale; // Menyimpan ukuran skala awal biji

    void Start()
    {
        mainCamera = Camera.main;
        raycastManager = FindObjectOfType<RaycastManager>(); // Cari instance RaycastManager
        inventoryManager = FindObjectOfType<InventoryManager>(); // Cari instance InventoryManager
    }

    void Update()
    {
        HandleDrag();
    }

    // Method berfungsi agar prefab biji yang ada pada Inventory dapat di drag
    // Interaksi menggunakan mouse atau touch (ditekan, digeser dan dilepas)
    // Method ini menggunakan referensi script
    // Inventory untuk menggunakan method (IsSeedInInventory)
    // Inventory untuk menggunakan method (RemoveSeedFromInventory)
    // CongklakHole method (AddSeed)
    // RaycastManager method (GetObjectUnderRaycast)
    // RaycastManager method (GetHoleUnderRaycast)
    private void HandleDrag()
    {
        // Deteksi touch/mouse down untuk memulai drag
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 inputPosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;

            GameObject hitObject = raycastManager.GetObjectUnderRaycast(inputPosition);

            if (hitObject != null)
            {
                // Periksa apakah biji ada di inventory
                if (inventoryManager.IsSeedInInventory(hitObject))
                {
                    selectedSeed = hitObject;

                    // Simpan posisi dan ukuran scale awal sebelum drag
                    initialPosition = selectedSeed.transform.localPosition;
                    initialScale = selectedSeed.transform.localScale;

                    // Hitung offset antara posisi input dan posisi objek
                    Vector3 worldPosition = GetWorldPositionFromInput(inputPosition);
                    offset = selectedSeed.transform.position - worldPosition;

                    // Mengubah ukuran biji saat di-drag
                    selectedSeed.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // Perbesar ukuran biji

                    Debug.Log("Mulai drag: " + selectedSeed.name);
                }
                else
                {
                    Debug.LogWarning("Biji ini belum berada di slot inventory!");
                }
            }
        }

        // Deteksi touch/mouse drag
        if (selectedSeed != null && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
        {
            Vector3 inputPosition = Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;

            Vector3 newPosition = GetWorldPositionFromInput(inputPosition) + offset;
            selectedSeed.transform.position = newPosition;
        }

        // Lepas drag saat mouse/touch dilepas
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (selectedSeed != null)
            {
                Vector3 inputPosition = Input.GetMouseButtonUp(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;

                // Panggil metode di RaycastManager untuk mendapatkan collider
                Collider hitCollider = raycastManager.GetHoleUnderRaycast(inputPosition);

                if (hitCollider != null)
                {
                    // Cek apakah collider adalah lubang yang valid
                    CongklakHole hole = hitCollider.GetComponent<CongklakHole>();
                    if (hole != null)
                    {
                        inventoryManager.RemoveSeedFromInventory(selectedSeed);
                        hole.AddSeed(selectedSeed);
                        selectedSeed.transform.localScale = new Vector3(1f, 1f, 1f);
                        Debug.Log("Biji dipindahkan ke lubang: " + hole.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("Collider bukan lubang yang valid.");
                    }
                }
                else
                {
                    // Kembalikan posisi jika tidak valid
                    selectedSeed.transform.localPosition = initialPosition;
                    selectedSeed.transform.localScale = initialScale;
                }

                // Reset selectedSeed
                selectedSeed = null;
            }
        }
    }

    // Fungsi untuk mendapatkan posisi dunia dari kursor atau menggerakkan prefab biji ketika di drag
    // Method ini digunakan pada method HandleDrag
    private Vector3 GetWorldPositionFromInput(Vector3 inputPosition)
    {
        inputPosition.z = Mathf.Abs(mainCamera.transform.position.z); // Jarak dari kamera ke objek
        return mainCamera.ScreenToWorldPoint(inputPosition);
    }
}