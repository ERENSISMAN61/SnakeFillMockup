using UnityEngine;

public class ClickHandler : MonoBehaviour
{

    [SerializeField] private LayerMask cylinderLayerMask;
    [SerializeField] private AttackSlotsController attackSlotsController;

    private int freeSlotIndex = -2;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left mouse button clicked.");
            freeSlotIndex = attackSlotsController.WhichSlotsFree();
            if (freeSlotIndex == -1) return;
            Debug.Log($"Free attack slot found at index: {freeSlotIndex}");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, cylinderLayerMask))
            {
                CylinderSpawner clickedCylinderSpawner = hitInfo.collider.GetComponent<CylinderSpawner>();
                if (clickedCylinderSpawner != null)
                {
                    clickedCylinderSpawner.ExportCylinder();
                }
            }
        }
    }
}
