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
                Cylinder clickedCylinder = hitInfo.collider.GetComponent<Cylinder>();
                if (clickedCylinder != null)
                {
                    Debug.Log($"Clicked on Cylinder with capacity: {clickedCylinder.capacity} and colorType: {clickedCylinder.colorType}");
                }
            }
        }
    }
}
