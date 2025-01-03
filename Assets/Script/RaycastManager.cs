using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public LayerMask holeLayer; // Layer untuk mendeteksi lubang congklak
    public LayerMask interactableDragLayer; // Layer untuk biji yang dapat diinteraksi
    private ColliderHoleManager colliderHoleManager;

    private void Start()
    {
        colliderHoleManager = FindObjectOfType<ColliderHoleManager>(); // Cari instance ColliderHoleManager
    }

    void Update()
    {
        TakeSeedToInventory();
        HandleRaycastColliderHole();
    }

    // Raycast untuk memasukkan biji ke inventory
    // Method ini menggunakan referensi script Congklak Hole untuk menggunakan method (HandleClick)
    public void TakeSeedToInventory()
    {
        // Deteksi input dari mouse atau touch
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 inputPosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;

            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            // Raycast hanya mendeteksi objek pada layer tertentu
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, holeLayer))
            {
                // Cek apakah objek memiliki script CongklakHole
                CongklakHole hole = hit.collider.GetComponent<CongklakHole>();
                if (hole != null)
                {
                    hole.HandleClick(); // Panggil logika lubang saat diklik
                    Debug.Log("Lubang diklik: " + hit.collider.gameObject.name);
                }
            }
        }
    }

    // Raycast untuk drag seed di inventory
    // Method ini digunakan pada script DragHandler (HandlerDrag - GetMouseDown/Touch Began)
    public GameObject GetObjectUnderRaycast(Vector3 inputPosition)
    {
        // Lakukan raycast
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableDragLayer))
        {
            Debug.Log("Raycast Seed");
            return hit.collider.gameObject; // Kembalikan objek yang terkena raycast
        }

        return null; // Tidak ada objek yang terkena
    }

    // Raycast untuk Memasukkan Seed ke Hole
    // Method ini digunakan pada script DragHandler (HandleDrag - GetMouseUp/Touch Ended)
    public Collider GetHoleUnderRaycast(Vector3 inputPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, holeLayer))
        {
            Debug.Log($"Raycast mengenai: {hit.collider.gameObject.name}");

            // Panggil ColliderHoleManager jika collider terdeteksi
            if (colliderHoleManager != null)
            {
                colliderHoleManager.OnColliderChoose(hit.collider);
            }

            // Pastikan objek yang terkena raycast memiliki komponen CongklakHole
            CongklakHole hole = hit.collider.GetComponent<CongklakHole>();
            if (hole != null)
            {
                // Kembalikan collider jika objek adalah CongklakHole
                return hit.collider;
            }
        }

        return null; // Tidak ada lubang yang terkena raycast
    }

    // Raycast untuk mengatur Collider pada Hole agar dapat (Aktif dan Nonaktif)
    // Digunakan pada method Update
    public void HandleRaycastColliderHole()
    {
        // Deteksi input dari mouse atau touch
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 inputPosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;

            // Lakukan raycast
            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, holeLayer))
            {
                // Panggil ColliderManager jika collider terdeteksi
                colliderHoleManager.OnColliderChoose(hit.collider);
            }
        }
    }
}