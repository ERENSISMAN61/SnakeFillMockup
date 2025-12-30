using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class CylinderSpawner : MonoBehaviour
{
    [SerializeField] private CylinderController cylinderController;
    [SerializeField] private AttackSlotsController attackSlotsController;

    [TableList]
    [SerializeField] private List<CylinderData> cylindersToSpawn = new List<CylinderData>();

    public List<GameObject> cylindersOnRoad = new List<GameObject>();


    void Start()
    {
        SpawnCylinders();
    }

    private void SpawnCylinders()
    {
        if (cylinderController == null || cylinderController.CylinderPrefab == null)
        {
            Debug.LogError("CylinderController or CylinderPrefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;
        float offset = cylinderController.OffsetZ;

        for (int i = 0; i < cylindersToSpawn.Count; i++)
        {
            Vector3 position = spawnPosition + new Vector3(0, 0, -offset * i);
            GameObject spawnedObject = Instantiate(cylinderController.CylinderPrefab, position, Quaternion.identity, transform);

            cylindersOnRoad.Add(spawnedObject);

            Cylinder cylinderComponent = spawnedObject.GetComponent<Cylinder>();
            if (cylinderComponent != null)
            {
                cylinderComponent.capacity = cylindersToSpawn[i].capacity;

                cylinderComponent.colorType = cylindersToSpawn[i].colorType;
                cylinderComponent.SetInitialText();
                cylinderComponent.SetTrashTransform(cylinderController.trashTransform);
                // Apply color based on ColorType
                Color color = ColorTypeProvider.GetColor(cylindersToSpawn[i].colorType);
                MeshRenderer meshRenderer = cylinderComponent.meshRenderer;
                if (meshRenderer != null)
                {
                    // Create material instance to avoid shared material modification
                    Material materialInstance = new Material(meshRenderer.material);
                    materialInstance.color = color;
                    meshRenderer.material = materialInstance;
                }

                cylinderComponent.cylinderSpawner = this;
            }
            else
            {
                Debug.LogWarning($"Spawned object does not have a Cylinder component!");
            }
        }
    }

    public void RemoveFirstItem()
    {
        if (cylindersOnRoad.Count > 0)
        {
            cylindersOnRoad.RemoveAt(0);
        }
    }
    public void ExportCylinder(int slotIndex)
    {
        if (attackSlotsController.attackSlots[slotIndex].isFulled) return;
        if (cylindersOnRoad.Count <= 0) return;

        Cylinder exportItem = null;
        exportItem = cylindersOnRoad[0].GetComponent<Cylinder>();

        if (exportItem == null) return;

        cylindersOnRoad.RemoveAt(0);

        // attackSlotsController.attackSlots[slotIndex].ImportCylinder(exportItem);



        exportItem.ExportItem(attackSlotsController, slotIndex);
    }





















}


[System.Serializable]
public class CylinderData
{
    public int capacity;
    public ColorType colorType;
}
