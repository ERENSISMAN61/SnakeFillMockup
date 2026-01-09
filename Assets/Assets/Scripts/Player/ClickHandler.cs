using UnityEngine;

public class ClickHandler : MonoBehaviour
{

    [SerializeField] private LayerMask cylinderLayerMask;
    [SerializeField] private Rail rail;
    [SerializeField] private GravityObject gravityObject;



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // freeSlotIndex = attackSlotsController.WhichSlotsFree();
            // if (freeSlotIndex == -1) return;


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, cylinderLayerMask))
            {
                CylinderSpawner clickedCylinderSpawner = hitInfo.collider.GetComponent<CylinderSpawner>();
                if (clickedCylinderSpawner != null)
                {
                    clickedCylinderSpawner.ExportCylinder(rail, gravityObject);
                }
            }
        }
    }
}
