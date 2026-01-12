using UnityEngine;

public class ClickHandler : MonoBehaviour
{

    [SerializeField] private LayerMask cylinderLayerMask;
    [SerializeField] private Rail rail;
    [SerializeField] private GravityObject gravityObject;
    [SerializeField] private float dragMultiplier = 3f;

    private CylinderSpawner currentCylinderSpawner;
    private GameObject draggedObject;
    private Vector3 lastMousePosition;
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, cylinderLayerMask))
            {
                currentCylinderSpawner = hitInfo.collider.GetComponent<CylinderSpawner>();
                if (currentCylinderSpawner != null)
                {
                    // İlk objeyi al
                    draggedObject = currentCylinderSpawner.GetFirstExportableObject();
                    if (draggedObject != null)
                    {
                        isDragging = true;
                        lastMousePosition = Input.mousePosition;
                    }
                }
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            // Mouse delta hesapla
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;

            // World space'e çevir ve 3x çarp
            Vector3 worldDelta = Camera.main.ScreenToWorldPoint(new Vector3(mouseDelta.x, mouseDelta.y, Camera.main.WorldToScreenPoint(draggedObject.transform.position).z))
                               - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.WorldToScreenPoint(draggedObject.transform.position).z));

            draggedObject.transform.position += worldDelta * dragMultiplier;

            lastMousePosition = currentMousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Export metodunu çalıştır
            if (currentCylinderSpawner != null)
            {
                currentCylinderSpawner.ExportCylinder(rail, gravityObject);
            }

            // Reset
            isDragging = false;
            draggedObject = null;
            currentCylinderSpawner = null;
        }
    }
}

// if (Input.GetMouseButtonDown(0))
// {
//     // freeSlotIndex = attackSlotsController.WhichSlotsFree();
//     // if (freeSlotIndex == -1) return;


//     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//     if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, cylinderLayerMask))
//     {
//         CylinderSpawner clickedCylinderSpawner = hitInfo.collider.GetComponent<CylinderSpawner>();
//         if (clickedCylinderSpawner != null)
//         {
//             clickedCylinderSpawner.ExportCylinder(rail, gravityObject);
//         }
//     }
// }
