using UnityEngine;
using DG.Tweening;
using TMPro;
public class Cylinder : MonoBehaviour
{
    public int capacity;
    public int UsedCapacity;

    public ColorType colorType;
    public MeshRenderer meshRenderer;

    public CylinderSpawner cylinderSpawner;
    public TextMeshPro capacityText;
    public GameObject bulletPrefab;

    private Transform trashTransform;


    public void ExportItem(AttackSlotsController attackSlotsController, int slotIndex)
    {
        transform.SetParent(trashTransform);
        // Logic to export this cylinder to the specified attack slot
        AttackSlot slot = attackSlotsController.attackSlots[slotIndex];
        if (!slot.isFulled)
        {
            slot.isFulled = true;
            ExportMove(slot.transform);
        }
    }


    public void ExportMove(Transform targetTransform)
    {
        transform.DOMove(targetTransform.position, 0.5f).OnComplete(() =>
        {

        });
    }

    public void SetInitialText()
    {
        capacityText.text = capacity.ToString();
    }
    public void SetTrashTransform(Transform trashTransform)
    {
        // Store the trash transform for later use
        this.trashTransform = trashTransform;
    }
}
